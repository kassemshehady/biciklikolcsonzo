using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using Bicikli_Admin.ApiModels;
using Bicikli_Admin.EntityFramework.linq;

namespace Bicikli_Admin.CommonClasses
{
    public class PrintingSubscription
    {
        /// <summary>
        /// Sets or Updates a printer subscription.
        /// </summary>
        /// <param name="printerModel"></param>
        /// <returns></returns>
        public static bool setPrinter(PrinterModel printerModel)
        {
            var dc = new BicikliDataClassesDataContext();
            int result = dc.SetPrinter(printerModel.lender_id, printerModel.printer_ip);

            if (result == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Removes the printer subscription from a lender.
        /// </summary>
        /// <param name="lender_id"></param>
        /// <returns></returns>
        public static bool removePrinter(int lender_id)
        {
            var printerModel = new PrinterModel();
            printerModel.lender_id = lender_id;
            printerModel.printer_ip = null;
            return setPrinter(printerModel);
        }

        /// <summary>
        /// This method throws an exception if the provided printer password
        /// does not match for the provided lender.
        /// </summary>
        /// <param name="lender_id"></param>
        /// <param name="printer_password"></param>
        public static void checkPassword(int lender_id, string printer_password)
        {
            var dc = new BicikliDataClassesDataContext();
            dc.Lenders.Single(l => ((l.id == lender_id) && (l.printer_password == printer_password)));
        }

        /// <summary>
        /// Sends an Invoice to a remote printer.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lender"></param>
        /// <returns></returns>
        public static bool sendInvoice(Session session, Lender lender)
        {
            var s = DataRepository.GetSession(session.id);
            var l = DataRepository.GetLender(lender.id);

            return sendInvoice(s, l);
        }

        /// <summary>
        /// Sends an Invoice to a remote printer.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lender"></param>
        /// <returns></returns>
        public static bool sendInvoice(Models.SessionModel session, Models.LenderModel lender)
        {
            if ((session.endTime == null) && (lender.printer_ip != null))
            {
                return false;
            }

            var invoice = new InvoiceModel()
            {
                address = (session.address ?? "-"),
                name = (session.name ?? "-"),
                lender_name = (lender.name ?? "-"),
                lender_address = (lender.address ?? "-"),
                total_balance = (int)(session.totalBalance),
                bike_name = (session.bikeModel.name ?? "-"),
                items = new List<InvoiceItemModel>()
            };

            invoice.items.Add(new InvoiceItemModel()
            {
                title = "Normál zónában töltött idő",
                vat = (float)session.normal_vat,
                base_unit_price = session.normal_price,
                amount = (int) Math.Round((double)(session.normalTimeMinutes ?? 0))
            });

            invoice.items.Add(new InvoiceItemModel()
            {
                title = "Veszélyes zónában töltött idő",
                vat = (float)session.danger_vat,
                base_unit_price = session.danger_price,
                amount = (int)Math.Round((double)(session.dangerousZoneTimeMinutes ?? 0))
            });

            try
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(lender.printer_ip), 6060);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    socket.Connect(remoteEP);
                    var stream = new StreamWriter(new NetworkStream(socket));
                    var serializer = new DataContractJsonSerializer(typeof(InvoiceModel));
                    using (MemoryStream memory = new MemoryStream())
                    {
                        serializer.WriteObject(memory, invoice);
                        stream.Write(Encoding.UTF8.GetString(memory.ToArray()));
                    }
                    stream.Flush();
                }
                catch
                {
                    try
                    {
                        var db = new BicikliDataClassesDataContext();
                        db.Lenders.Single(l => l.id == lender.id).printer_ip = null;
                        db.SubmitChanges();
                    }
                    catch { }

                    return false;
                }

                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch { }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}