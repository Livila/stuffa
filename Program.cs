﻿using System;
using System.Windows;
using Stuffa;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SocketServer
{
    public class Handler : WebSocketBehavior
    {

        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("--------------------------");
            Console.WriteLine(e.Data.ToString());
            var msg = e.Data.ToString().ToUpper();
            Server.pausePlay();
            
            Send(msg);
        }

        protected override void OnOpen() => Console.WriteLine("opened");

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("connection closed");
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Console.WriteLine("error");
        }

        public void test(){

        }
    }

    public class Server
    {
        static MainWindow mainWin;
        //public static void Main(string[] args)
        public static void init(MainWindow mainArg)
        {
            mainWin = mainArg;


            var wssv = new WebSocketServer(8080);
            wssv.AddWebSocketService<Handler>("/remote");
            
            wssv.Start();
            Console.WriteLine($"Listening on {wssv.Address}:{wssv.Port}");
            foreach (var path in wssv.WebSocketServices.Paths)
                Console.WriteLine("- {0}", path);
            //Console.ReadKey(true);
            //wssv.Stop();
        }


        public static void pausePlay()
        {
            mainWin.pausePlay();
        }

    }
}