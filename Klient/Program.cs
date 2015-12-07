using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Klient
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO: 1. Utworzenie punktu połączenia do serwera - IPEndPoint
            var punktSerwer = new IPEndPoint(IPAddress.Parse("192.168.56.1"), 2000);
            //TODO: 2. Utworzenie gniazda połączenia - Socket
            var gniazdoSerwer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //TODO: 3. Połączenie z serwerem - Socket.Connect
            gniazdoSerwer.Connect(punktSerwer); //połączenie z serwerem
            //TODO: 4. Pobranie danych z serwera - Socket.Receive(ASCIIEncoding)       
            var tablicaDane = new byte[64]; //Bajt
            gniazdoSerwer.Receive(tablicaDane);  //Oczekiwanie i pobranie danych
            Console.WriteLine(Encoding.UTF8.GetString(tablicaDane));

            var watek = new Thread(WysylanieCzat); //wątek dla nadłuchiwania z klawiatury i wysyłania do serwera
            watek.IsBackground = true;
            watek.Start(gniazdoSerwer);

            while (true) //pętla dla obierania wiadomości
            {
                var dane = new byte[512]; //256 -> 3 znaki + 253 spacje
                gniazdoSerwer.Receive(dane);
                Console.WriteLine(Encoding.UTF8.GetString(dane));
            }
            //Kod - klient równocześnie wysyła i odbiera komunikaty           
        }

        static void WysylanieCzat(object polaczenieSerwer) //metoda uruchamiana w osobnym wątku
        {
            var polaczenieZSerwerem = (Socket)polaczenieSerwer;

            while (true)
            {
                var komunikat = Console.ReadLine(); //pobranie tekstu z klawiatury - do entera
                polaczenieZSerwerem.Send(Encoding.UTF8.GetBytes(komunikat)); //wysłanie komunikatu do serwera
            }
        }
    }
}
