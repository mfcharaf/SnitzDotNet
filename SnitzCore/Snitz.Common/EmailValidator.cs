using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using Bdev.Net.Dns;
using SnitzConfig;

namespace SnitzCommon
{
    public static class EmailValidator
    {
        public static string IsValidEmail(string email)
        {
            string mailserver = "";
            TcpClient tClient = null;

            var server = EmailValidator.GetMXServer(email);
            if (server.ToString().StartsWith("MX"))
            {
                mailserver = server.ToString().Replace("MX:Mail Server =", "").Split(',')[0].Trim();
            }

            if (mailserver == "")
                return "Invalid mailserver";

            try
            {
                tClient = new TcpClient(mailserver, 25);
                const string crlf = "\r\n";

                NetworkStream netStream = tClient.GetStream();

                GetResponse(netStream);
                /* Perform HELO to SMTP Server and get Response */
                byte[] dataBuffer = BytesFromString("HELO " + Config.EmailHost + crlf);
                netStream.Write(dataBuffer, 0, dataBuffer.Length);

                GetResponse(netStream); 

                dataBuffer = BytesFromString("MAIL FROM:<" + Config.AdminEmail + ">" + crlf);
                netStream.Write(dataBuffer, 0, dataBuffer.Length);
                GetResponse(netStream); 

                /* Read Response of the RCPT TO Message to know if it exist or not */
                dataBuffer = BytesFromString("RCPT TO:<" + email + ">" + crlf);
                netStream.Write(dataBuffer, 0, dataBuffer.Length);
                var responseString = GetResponse(netStream);

                if (GetResponseCode(responseString) == 550)
                {
                    return responseString;
                }

                /* QUIT CONNECTION */
                dataBuffer = BytesFromString("QUIT" + crlf);
                netStream.Write(dataBuffer, 0, dataBuffer.Length);
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (tClient != null)
                {
                    tClient.Close();
                }
            }

            
            return "OK";
        }

        private static string GetResponse(NetworkStream netStream)
        {
            byte[] myReadBuffer = new byte[1024];
            StringBuilder myCompleteMessage = new StringBuilder();
            do
            {
                int numberOfBytesRead = netStream.Read(myReadBuffer, 0, myReadBuffer.Length);
                myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
            }
            while (netStream.DataAvailable);

            return myCompleteMessage.ToString();
        }

        private static byte[] BytesFromString(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }
        private static int GetResponseCode(string responseString)
        {
            return int.Parse(responseString.Substring(0, 3));
        }

        private static StringBuilder GetMXServer(string email)
        {
            StringBuilder output = new StringBuilder();
            var nameservers = GetDNSServerAddress();
            //foreach (string nameserver in nameservers)
            //{
            //    output.AppendLine(nameserver);
            //}
            string ip = nameservers[0];
            IPAddress dnsServer = IPAddress.Parse(ip);

            return Query(dnsServer, email.Split('@')[1], DnsType.MX);
        }
        private static StringBuilder Query(IPAddress dnsServer, string domain, DnsType type)
        {
            StringBuilder output = new StringBuilder();
            try
            {
                // create a DNS request
                Request request = new Request();

                // create a question for this domain and DNS CLASS
                request.AddQuestion(new Question(domain, type, DnsClass.IN));

                // send it to the DNS server and get the response
                Response response = Resolver.Lookup(request, dnsServer);

                // check we have a response
                if (response == null)
                {
                    output.AppendLine("No answer");
                    return output;

                }
                // display whether this is an authoritative answer or not
                //output.AppendLine(response.AuthoritativeAnswer ? "authoritative answer" : "Non-authoritative answer");

                // Dump all the records - answers/name servers/additional records
                foreach (Answer answer in response.Answers)
                {
                    output.AppendFormat("{0}:{1}", answer.Type, answer.Record).AppendLine("");
                }

                //foreach (NameServer nameServer in response.NameServers)
                //{
                //    output.AppendFormat("{0}:{1}", nameServer.Type, nameServer.Record).AppendLine("");
                //}

                //foreach (AdditionalRecord additionalRecord in response.AdditionalRecords)
                //{
                //    output.AppendFormat("{0}:{1}", additionalRecord.Type, additionalRecord.Record).AppendLine("");
                //}
            }
            catch (Exception ex)
            {
                output.AppendLine(ex.Message);
            }

            return output;
        }
        private static string[] GetDNSServerAddress()
        {
            List<string> nameservers = new List<string>();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in nics)
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    IPAddressCollection ips = ni.GetIPProperties().DnsAddresses;

                    foreach (System.Net.IPAddress ip in ips)
                    {
                        nameservers.Add(ip.ToString());
                    }
                }
            }
            return nameservers.ToArray();
        }

    }
}
