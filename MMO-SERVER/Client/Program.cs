﻿using System;
using System.Threading;
using Serilog;
using System.IO;
using System.Collections.Generic;
using lLua.Binchunk;
using Common.Summer.Security;
using Serilog.Sinks.SystemConsole.Themes;
using Common.Summer.Net;
using HS.Protobuf.Login;
using Google.Protobuf;
using Common.Summer.Core;
using HS.Protobuf.LoginGate;
using HS.Protobuf.Common;
using Common.Summer.MyLog;
using Common.Summer.Server;

namespace ClientTest
{
    class Program
    {
        static void TestAES()
        {
            var pair = AesEncryption.GenerateAesKeyAndIv();
            AesEncryption aes = new AesEncryption(pair.Key, pair.IV);

            string original = "Hello, World!111111111111111111111111111111111111111111111111111111111111111111111111111111111Hello, World!111111111111111111111111111111111111111111111111111111111111111111111111111111111Hello, World!111111111111111111111111111111111111111111111111111111111111111111111111111111111Hello, World!111111111111111111111111111111111111111111111111111111111111111111111111111111111Hello, World!111111111111111111111111111111111111111111111111111111111111111111111111111111111" +
                "Hello, World!111111111111111111111111111111111111111111111111111111111111111111111111111111111Hello, World!111111111111111111111111111111111111111111111111111111111111111111111111111111111Hello, World!111111111111111111111111111111111111111111111111111111111111111111111111111111111Hello, World!111111111111111111111111111111111111111111111111111111111111111111111111111111111" +
                "Hello, World!111111111111111111111111111111111111111111111111111111111111111111111111111111111Hello, World!111111111111111111111111111111111111111111111111111111111111111111111111111111111Hello, World!111111111111111111111111111111111111111111111111111111111111111111111111111111111Hello, World!111111111111111111111111111111111111111111111111111111111111111111111111111111111";
            Console.WriteLine("Original:   " + original);

            string encrypted = aes.Encrypt(original);
            Console.WriteLine("Encrypted:  " + encrypted);

            string decrypted = aes.Decrypt(encrypted);
            Console.WriteLine("Decrypted:  " + decrypted);
        }

        static void TestRSA()
        {
            RsaEncryption rsaEncryption = new RsaEncryption();

            // 获取公钥和私钥
            string publicKey = rsaEncryption.GetPublicKey();
            string privateKey = rsaEncryption.GetPrivateKey();

            Console.WriteLine("Public Key: " + publicKey);
            Console.WriteLine("Private Key: " + privateKey);

            var pair = AesEncryption.GenerateAesKeyAndIv();

            // 加密数据
            string originalText = pair.Key;
            Console.WriteLine("\nOriginal: " + pair.Key);

            string encryptedText = rsaEncryption.Encrypt(originalText);
            Console.WriteLine("Encrypted: " + encryptedText);

            // 解密数据
            string decryptedText = rsaEncryption.Decrypt(encryptedText);
            Console.WriteLine("Decrypted: " + decryptedText);
        }

        static void TestRsa2()
        {
            EncryptionManager encryptionManager1 = new();
            encryptionManager1.Init();
            EncryptionManager encryptionManager2 = new();
            encryptionManager2.Init();

            string originalText = "hello world111111111111111111111111111111111111111111111111111world111111111111111111111111111111111111111111111111111world111111111111111111111111111111111111111111111111111" +
                "world111111111111111111111111111111111111111111111111111world111111111111111111111111111111111111111111111111111world1111111" +
                "1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111";
            Console.WriteLine("Original: " + originalText);

            encryptionManager1.SetRemoteRsaPublicKey(encryptionManager2.GetLocalRsaPublicKey());
            string encryptedText = encryptionManager1.RsaEncrypt(originalText);
            Console.WriteLine("Encrypted: " + encryptedText);

            string decryptedText = encryptionManager2.RsaDecrypt(encryptedText);
            Console.WriteLine("Decrypted: " + decryptedText);

        }

        static void TestHash()
        {
            // 示例密码
            string password = "my_secure_password";

            // 哈希密码
            string hashedPassword = PasswordHasher.Instance.HashPassword(password);
            Console.WriteLine($"Hashed Password: {hashedPassword}");

            // 验证密码
            bool isPasswordCorrect = PasswordHasher.Instance.VerifyPassword("my_secure_password", hashedPassword);
            Console.WriteLine($"Is the password correct? {isPasswordCorrect}");

            // 尝试使用错误的密码进行验证
            bool isWrongPasswordCorrect = PasswordHasher.Instance.VerifyPassword("wrong_password", hashedPassword);
            Console.WriteLine($"Is the wrong password correct? {isWrongPasswordCorrect}");
        }


        static void Lua()
        {
            string fileName = "luac.out";

            // 获取当前执行的可执行文件所在的目录
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory;

            // 构建完整的文件路径
            string filePath = Path.Combine(directoryPath, fileName);

            try
            {
                // 读取文件内容
                byte[] fileBytes = File.ReadAllBytes(filePath);
                var p = Binary_Chunk.Undump(fileBytes);
                Binary_Chunk.Print(p);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static void Main(string[] args)
        {
            SerilogManager.Instance.Init();
            // Thread.Sleep(2000);

            //ProtoHelper.Register<UserLoginRequest>((int)LoginProtocl.UserLoginRequest);
            //ProtoHelper.Register<UserLoginResponse>((int)LoginProtocl.UserLoginResponse);
            //ProtoHelper.Register<GetLoginGateTokenRequest>((int)LoginGateProtocl.GetLogingateTokenReq);
            //ProtoHelper.Register<GetLoginGateTokenResponse>((int)LoginGateProtocl.GetLogingateTokenResp);

            //MessageRouter.Instance.Start(1);
            //MessageRouter.Instance.Subscribe<UserLoginResponse>(_HandleUserLoginResponse);
            //MessageRouter.Instance.Subscribe<GetLoginGateTokenResponse>(_HandleGetLoginGateTokenResponse);


            //m_netClient = new NetClient();
            //m_netClient.Init("127.0.0.1", 10700, 10,
            //    (netClient) => { 
            //        Log.Debug("Connected to LoginGate Server.");
            //    },
            //    (tcpClient, isEnd) => { },
            //    (tcpClient) => { });

            // TestHash();

            TestServerSelector();


            Console.ReadLine();
        }

        private static void TestServerSelector()
        {
            var server1 = new ServerInfoNode
            {
                ServerId = 1,
            };
            var server2 = new ServerInfoNode
            {
                ServerId = 2,
            };
            var server3 = new ServerInfoNode
            {
                ServerId = 3,
            };
            List<ServerInfoNode> list = new List<ServerInfoNode>();
            list.Add(server1);
            list.Add(server2);
            list.Add(server3);

            var selector = new ServerSelector(list, ServerSelector.SelectionStrategy.Random);
            var serverInfoNode =  selector.SelectServer();
            Log.Information(serverInfoNode.ToString());
            serverInfoNode = selector.SelectServer();
            Log.Information(serverInfoNode.ToString());
            serverInfoNode = selector.SelectServer();
            Log.Information(serverInfoNode.ToString());
            serverInfoNode = selector.SelectServer();
            Log.Information(serverInfoNode.ToString());
        }

        private static NetClient m_netClient;
        private static string loginGateToken;

        private static void _HandleGetLoginGateTokenResponse(Connection sender, GetLoginGateTokenResponse message)
        {
            loginGateToken = message.LoginGateToken;
            IMessage userLoginRequest = new UserLoginRequest { Username = "令狐冲", Password = "123" , LoginGateToken = loginGateToken };
            m_netClient.Send(userLoginRequest);
        }

        private static void _HandleUserLoginResponse(Connection sender, UserLoginResponse message)
        {
            Log.Information(message.ToString());  
        }




    }
}
