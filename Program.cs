
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace bugiWOL
{
    class Program
    {
        //string2mac
        public static byte[] GetMacArray(string mac)
        {
            if (string.IsNullOrEmpty(mac)) throw new ArgumentNullException("mac");
            byte[] ret = new byte[6];
            try
            {
                string[] tmp = mac.Split(':', '-');
                if (tmp.Length != 6)
                {
                    tmp = mac.Split('.');
                    if (tmp.Length == 3)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            ret[i * 2] = byte.Parse(tmp[i].Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                            ret[i * 2 + 1] = byte.Parse(tmp[i].Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                        }
                    }
                    else
                        for (int i = 0; i < 12; i += 2)
                            ret[i / 2] = byte.Parse(mac.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                }
                else
                    for (int i = 0; i < 6; i++)
                        ret[i] = byte.Parse(tmp[i], System.Globalization.NumberStyles.HexNumber);
            }
            catch
            {
                return new byte[] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
            }
            return ret;
        }

        //send WOL for MAC
        private static void WakeOnLan(byte[] mac)
        {
            // WOL packet is sent over UDP 255.255.255.0:40000.
            UdpClient client = new UdpClient();
            client.Connect(IPAddress.Broadcast, 40000);

            // WOL packet contains a 6-bytes trailer and 16 times a 6-bytes sequence containing the MAC address.
            byte[] packet = new byte[17 * 6];

            // Trailer of 6 times 0xFF.
            for (int i = 0; i < 6; i++)
                packet[i] = 0xFF;

            // Body of magic packet contains 16 times the MAC address.
            for (int i = 1; i <= 16; i++)
                for (int j = 0; j < 6; j++)
                    packet[i * 6 + j] = mac[j];

            // Send WOL packet.
            client.Send(packet, packet.Length);
        }

        //check if has to start and send WOL
        static void startWOL()
        {
            //get files containing WOL-dates
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\MAC\", "wol.*");

            //foreach MAC do
            foreach (string file in files)
            {
                //get MAC adress
                string mac = file.Replace(Directory.GetCurrentDirectory() + @"\MAC\", "").Replace("wol.", "");

                DateTime now = DateTime.Now;

                //search for: file contains date of now
                using (StreamReader sr = File.OpenText(file))
                {
                    string[] lines = File.ReadAllLines(file);
                    foreach (string line in lines)
                    {
                        //if file contains
                        if (line.Contains(now.ToString("dd.MM.yy")))
                        {
                            sr.Close();
                            //send WakeOnLan
                            WakeOnLan(GetMacArray(mac));
                        }
                    }
                }
            }
        }

        //create lists containing dates, where MAC has to been started
        static void createLists()
        {
            //get all files containing "files."
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\MAC\", "ferien.*");

            //foreach create the list
            foreach (string file in files)
            {
                bool started = false;

                //init
                DateTime now = DateTime.Parse("01.01.2100");
                DateTime untilDate = DateTime.Parse("01.01.2100");
                DateTime beginAfterDate = DateTime.Parse("01.01.2100");

                //get filename to save to
                string target = file.Replace("ferien", "wol");
                if (File.Exists(target))
                    File.Delete(target);
                File.CreateText(target).Close();

                //read all entries in "ferien."-file
                foreach(string date in File.ReadAllLines(file))
                {
                    //get startDate
                    if (started == false)
                    {
                        DateTime startDate = DateTime.Parse(date);
                        now = startDate;
                        started = true;
                    }
                    else 
                    {
                        //is ignoreStart,ignoreEnd
                        if (date.Contains(',') == true)
                        {
                            string[] span = date.Split(',');
                            untilDate = DateTime.Parse(span[0]);
                            beginAfterDate = DateTime.Parse(span[1]);
                        }
                        //is lastEntry
                        else
                        {
                            untilDate = DateTime.Parse(date).AddDays(1);
                        }

                        //fill file to save to with content until next ignoreStart
                        while (now < untilDate)
                        {
                            //not on Saturday and Sunday
                            if (now.DayOfWeek != DayOfWeek.Saturday && now.DayOfWeek != DayOfWeek.Sunday)
                            {
                                //write content
                                File.AppendAllText(target, now.ToString("dd.MM.yy") + Environment.NewLine);
                            }
                            //nextDay
                            now = now.AddDays(1);
                        }
                        //last day is an active day
                        now = beginAfterDate.AddDays(1);
                    }
                }                
            }
        }

        //main
        static void Main(string[] args)
        {
            bool withParam = false;

            //check input arguments
            foreach (string  arg in args)
            {
                if (arg.Contains("create"))
                {
                    createLists();
                }

                withParam = true;
            }

            //on standard start WOL
            if (!withParam)
            {
                startWOL();
            }

            //Console.ReadLine();
        }
    }
}
