using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace LayoutTest
{
    class SocketConnection
    {
        Socket socketServer;
        byte[] tmpData = new byte[1000 * 1024];
        public Action<string> msg_callback;


        public SocketConnection()
        {
            try
            {
                socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socketServer.Bind(new IPEndPoint(IPAddress.Any, 5656));
                socketServer.Listen(int.MaxValue);

                Msg("server on line,wait for connection...");
            }
            catch (Exception ex)
            {
                Msg(ex);
            }

            socketServer.BeginAccept((ar) =>
            {
                AcceptCallback(ar);
            }, socketServer);
        }
        void Msg(string msg)
        {
            Console.WriteLine(msg);
            if (msg_callback != null)
            {
                msg_callback(msg);
            }

        }
        void Msg(Exception msg)
        {
            Console.WriteLine(msg);
            if (msg_callback != null)
            {
                msg_callback(msg.ToString());
            }

        }


        public ServerCommand ParseHeader(byte[] raw)
        {
            byte[] packageHeader = new byte[4];

            packageHeader[0] = raw[0];
            packageHeader[1] = raw[1];
            packageHeader[2] = raw[2];
            packageHeader[3] = raw[3];
            return (ServerCommand)BitConverter.ToInt32(packageHeader, 0);
        }
        public byte[] UnPack(byte[] raw)
        {
            byte[] dest = new byte[raw.Length - 4];
            Buffer.BlockCopy(raw, 4, dest, 0, raw.Length - 4);

            return dest;
        }

    

        void DealPackage(Socket ts, byte[] body_data)
        {
            CommData dataSave = new CommData() { bodyLength = body_data.Length, bodyData = body_data };
            switch (ParseHeader(dataSave.bodyData))
            {
            }
        }
        void ReceivePackage(Socket ts, int receiveLength, byte[] receiveBuffer, int needBodyLength, byte[] allocBuffer)
        {
            if (needBodyLength > receiveLength - 4)//不完整包
            {
                Buffer.BlockCopy(receiveBuffer, 4, allocBuffer, 0, receiveLength - 4);//把所有数据放进Buffer,不包含包的前4字节
                int fillLength = receiveLength - 4;

                while (true)
                {
                    //收到包截断的情况，继续接收
                    if (fillLength != needBodyLength)
                    {
                        int body_part = ts.Receive(tmpData, 0, tmpData.Length, SocketFlags.None);

                        if (fillLength + body_part > needBodyLength)
                        {
                            //粘包
                            int visioLength = (fillLength + body_part) - needBodyLength;//粘包长度

                            Buffer.BlockCopy(tmpData, 0, allocBuffer, fillLength, body_part - visioLength);
                            DealPackage(ts, allocBuffer);
                            byte[] visioBuffer = new byte[visioLength];
                            Buffer.BlockCopy(tmpData, needBodyLength - fillLength, visioBuffer, 0, visioLength);
                            VisioPackage(ts, visioBuffer);
                            break;
                        }
                        Buffer.BlockCopy(tmpData, 0, allocBuffer, fillLength, body_part);

                        fillLength += body_part;
                    }
                    else
                    {
                        DealPackage(ts, allocBuffer);
                        break;
                    }
                }

            }
            else if (needBodyLength == receiveLength - 4)
            {
                Buffer.BlockCopy(receiveBuffer, 4, allocBuffer, 0, needBodyLength);
                DealPackage(ts, allocBuffer);
            }
            else//粘包
            {
                Buffer.BlockCopy(receiveBuffer, 4, allocBuffer, 0, needBodyLength);
                DealPackage(ts, allocBuffer);
                int visioLength = receiveLength - (needBodyLength + 4);//粘包长度
                byte[] visioBuffer = new byte[visioLength];

                Buffer.BlockCopy(receiveBuffer, 4 + needBodyLength, visioBuffer, 0, visioLength);
                VisioPackage(ts, visioBuffer);
            }
        }

        void VisioPackage(Socket ts, byte[] visioBuffer)
        {
            if (visioBuffer.Length >= 4)//can read package size
            {
                byte[] packageHeader = new byte[4];
                packageHeader[0] = visioBuffer[0];
                packageHeader[1] = visioBuffer[1];
                packageHeader[2] = visioBuffer[2];
                packageHeader[3] = visioBuffer[3];
                int bodyLength = BitConverter.ToInt32(packageHeader, 0);
                byte[] bodyData = new byte[bodyLength];

                if (visioBuffer.Length >= bodyLength + 4)
                {
                    ReceivePackage(ts, visioBuffer.Length, visioBuffer, bodyLength, bodyData);
                }
                else
                {
                    int body_part = ts.Receive(tmpData, 0, tmpData.Length, SocketFlags.None);
                    byte[] receiveBuffer = new byte[visioBuffer.Length + body_part];

                    Buffer.BlockCopy(visioBuffer, 0, receiveBuffer, 0, visioBuffer.Length);
                    Buffer.BlockCopy(tmpData, 0, receiveBuffer, visioBuffer.Length, body_part);

                    ReceivePackage(ts, receiveBuffer.Length, receiveBuffer, bodyLength, bodyData);
                }

            }
            else//cant read package size,must read next pocket
            {
                int body_part = ts.Receive(tmpData, 0, tmpData.Length, SocketFlags.None);
                byte[] receiveBuffer = new byte[visioBuffer.Length + body_part];

                Buffer.BlockCopy(visioBuffer, 0, receiveBuffer, 0, visioBuffer.Length);
                Buffer.BlockCopy(tmpData, 0, receiveBuffer, visioBuffer.Length, body_part);

                byte[] packageHeader = new byte[4];
                packageHeader[0] = receiveBuffer[0];
                packageHeader[1] = receiveBuffer[1];
                packageHeader[2] = receiveBuffer[2];
                packageHeader[3] = receiveBuffer[3];
                int bodyLength = BitConverter.ToInt32(packageHeader, 0);
                byte[] bodyData = new byte[bodyLength];

                ReceivePackage(ts, receiveBuffer.Length, receiveBuffer, bodyLength, bodyData);
            }
        }
        void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                Socket ts = (Socket)result.AsyncState;
                int c = ts.EndReceive(result);

                result.AsyncWaitHandle.Close();
                if (c == 0)
                {
                    ts.Disconnect(false);
                }
                else
                {
                    byte[] packageHeader = new byte[4];
                    packageHeader[0] = tmpData[0];
                    packageHeader[1] = tmpData[1];
                    packageHeader[2] = tmpData[2];
                    packageHeader[3] = tmpData[3];

                    int bodyLength = BitConverter.ToInt32(packageHeader, 0);
                    byte[] bodyData = new byte[bodyLength];
                    ReceivePackage(ts, c, tmpData, bodyLength, bodyData);
                    ts.BeginReceive(tmpData, 0, tmpData.Length, SocketFlags.None, ReceiveCallback, ts);
                }
            }
            catch (Exception ex)
            {
                Msg(ex);
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                socketServer.BeginAccept((ar2) =>
                {
                    AcceptCallback(ar2);

                }, socketServer);


                Socket ts = socketServer.EndAccept(ar);
                ar.AsyncWaitHandle.Close();
                Msg("one client has connect");

                ts.BeginReceive(tmpData, 0, tmpData.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), ts);

            }
            catch (Exception e)
            {
                Msg(e);
            }
        }

    }
}
