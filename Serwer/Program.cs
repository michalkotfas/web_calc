using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Serwer
{
    class Program
    {
        static List<Socket> klienciCzat = new List<Socket>(); //lista klientów - czyt. gniazd

        static void Main(string[] args)
        {
            string adresIp = "192.168.56.1"; //podajemy adres ip naszezgo komputera na którym pracujemy trzeba go sprawdzic w cmd poprzez polecenie ipconfig następnie przepisać adres z ipv4 Address

            //TODO: 1. Utworzenie punktu nadsłuchiwania - IPEndPoint
            var punktNadsluchiwanie = new IPEndPoint(IPAddress.Parse(adresIp), 2000);
            //TODO: 2. Utworzenie gniazda nadsłuchiwania - Socket
            var gniazdo = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //TODO: 3. Przypisanie gniazda do adresu IP - Socket.Bind
            gniazdo.Bind(punktNadsluchiwanie);
            //TODO: 4. Rozpoczęcie nadsłuchiwania - Socket.Listen
            gniazdo.Listen(10);

            while (true)//TODO: 5. W pętli oczekiwanie na połączenia - Socket.Accept
            {
                var polaczenieZKlientem = gniazdo.Accept(); //oczekiwanie na połączenia
                klienciCzat.Add(polaczenieZKlientem); //dodanie nowego klienta do listy
                var danePolaczenie = string.Format("Port serwer: {0}; Klient: {1}:{2}",
                   ((IPEndPoint)polaczenieZKlientem.LocalEndPoint).Port,
                   ((IPEndPoint)polaczenieZKlientem.RemoteEndPoint).Address.ToString(),
                   ((IPEndPoint)polaczenieZKlientem.RemoteEndPoint).Port);
                Console.WriteLine(danePolaczenie);
                var watek = new Thread(KlientCzat);
                watek.IsBackground = true;
                watek.Start(polaczenieZKlientem);
            }
        }

        static void KlientCzat(object polaczenieKlient) //metoda uruchamiana w osobnym wątku
        {
            var polaczenieZKlientem = (Socket)polaczenieKlient;
            //TODO: 6. Przesłanie danych klientowi - Socket.Send(ASCIIEncoding)
            polaczenieZKlientem.Send(Encoding.UTF8.GetBytes("Witaj, podaj bok A i wysokość np: 2,3"));

            while (true)
            {
                var dane = new byte[512]; //256 -> 3 znaki + 253 spacje
                polaczenieZKlientem.Receive(dane); //pobranie danych

                foreach (var klient in klienciCzat)
                {
                    klient.Send(Encoding.ASCII.GetBytes(poleTrojkata(System.Text.Encoding.UTF8.GetString(dane))));
                }
            }
        }

        //Metoda obliczająca pole trójkąta//
        static string poleTrojkata(string dane)
        {
            string[] tablicaDanych = dane.Split(',');
            double aBok = double.Parse(tablicaDanych[0]);
            double hWysokosc = double.Parse(tablicaDanych[1]);

            double wynik = (aBok * hWysokosc) / 2;

            return "Pole trojkata z boku A rownego " + aBok.ToString() + " oraz wysokosci rownej " + hWysokosc.ToString() + " rowna sie " + wynik.ToString();
        }
    }
}
