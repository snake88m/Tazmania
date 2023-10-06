using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;
using Tazmania.Helpers;
using Tazmania.Interfaces.Automation;

namespace Tazmania.Automation
{
    public class DuemmegiService : IDuemmegiService
    {
        public async Task ReadIO(IEnumerable<IO> ios)
        {
            using (TcpClient tcpClient = new TcpClient("192.168.1.252", 80))
            {
                tcpClient.SendTimeout = 500;
                tcpClient.ReceiveTimeout = 500;

                using (NetworkStream networkStream = tcpClient.GetStream())
                {
                    foreach (int dfcpAddress in ios.Select(r => r.DFCPAddress))
                    {
                        List<byte> message = new List<byte>();

                        // indirizzo del nodo DFCP (0 vuol dire leggi su tutti i nodi)
                        message.Add(0);

                        // comando di lettura memoria ram
                        message.Add(0x7F);

                        // byte totali da cui verrà composta la parte dati della trama
                        message.Add(4);

                        // indirizzo di 3 bytes della memoria ram da cui iniziare a leggere
                        message.AddRange(ProtocolHelper.ConvertToBytes(dfcpAddress, 3));

                        // numero byte totali da leggere dalla memoria
                        message.Add(2);

                        // calcolo il checksum di 2 bytes
                        message.AddRange(ProtocolHelper.CalculateCHSAt1(message.ToArray()));

                        byte[] response = new byte[7];

                        await networkStream.WriteAsync(message.ToArray(), 0, message.Count);
                        await networkStream.ReadAsync(response, 0, response.Length);
                        
                        foreach (IO io in ios.Where(r => r.DFCPAddress == dfcpAddress))
                        {
                            if (io.Type == IOType.Output)
                            {
                                io.IsActive = ProtocolHelper.GetBit(response[3], io.ModulePin);
                            }
                            else if (io.Type == IOType.Input)
                            {
                                bool newStatus = ProtocolHelper.GetBit(response[3], io.ModulePin);

                                if (io.IsInverted)
                                {
                                    newStatus = !newStatus;
                                }

                                // se lo stato è passato da off => on
                                if (!io.IsActive && newStatus)
                                {
                                    // resetto il flag di handled e il datetime di rilascio
                                    // aggiorno il datetime di inizio
                                    io.Input.Handled = false;
                                    io.Input.StartAction = DateTime.Now;
                                    io.Input.EndAction = DateTime.MinValue;
                                }

                                //se lo stato è passato da on => off
                                if (io.IsActive && !newStatus)
                                {
                                    // aggiorno il datetime di rilascio
                                    io.Input.EndAction = DateTime.Now;
                                }

                                io.IsActive = newStatus;
                            }
                            else if (io.Type == IOType.InputValue)
                            {
                                // 256 * high + low => faccio la somma binaria della word
                                // (word - 2730) / 10 => formula matematica DFCP per ottenere il valore corretto
                                float newValue = ((256 * response[3]) + response[4] - 2730f) / 10;

                                if (io.Value != newValue)
                                {
                                    // resetto il flag di handled
                                    // aggiorno il datetime di inizio (il datetime di fine non è applicabile)
                                    io.Input.Handled = false;
                                    io.Input.StartAction = DateTime.Now;
                                }

                                io.Value = newValue;
                            }
                        }
                    }
                }
            }
        }

        public async Task WriteOutput(IEnumerable<IO> outputs)
        {
            if (outputs.Any(r => r.Type != IOType.Output))
            {
                throw new ArgumentException("Only output allowed", nameof(outputs));
            }

            List<byte[]> messages = new List<byte[]>();

            foreach (int dfcpAddress in outputs.Select(r => r.DFCPAddress))
            {
                if (outputs.Any(r => r.DFCPAddress == dfcpAddress && !r.Output.Handled && !r.HasVirtual))
                {
                    List<byte> message = new List<byte>();

                    // indirizzo del nodo DFCP (0 vuol dire leggi su tutti i nodi)
                    message.Add(0);

                    // comando di scrittura su memoria ram
                    message.Add(0x7E);

                    // byte totali da cui verrà composta la parte dati della trama
                    message.Add(5);

                    // indirizzo di 3 bytes della memoria ram da cui iniziare a scrivere
                    message.AddRange(ProtocolHelper.ConvertToBytes(dfcpAddress, 3));

                    // numero byte totali da scrivere in memoria
                    message.Add(1);

                    byte newValue = 0;

                    // calcolo il valore binario di tutti gli output collegati al modulo
                    foreach (var output in outputs.Where(r => r.DFCPAddress == dfcpAddress))
                    {
                        // se c'è un Parent assegnato i due dispositivi non possono essere accesi insieme
                        // quindi se ciò si verifica imposto il nuovo stato in Handled
                        if (output.ParentId.HasValue && (!output.Output.Handled && output.Output.NewStatus))
                        {
                            IO parentOutput = outputs.Single(r => r.Id == output.ParentId);

                            if (parentOutput.IsActive || (!parentOutput.Output.Handled && parentOutput.Output.NewStatus))
                            {
                                Log.Information($"Cambio stato disattivo per output {output.Id} {output.Description}. Parent attualmente attivo {parentOutput.Id} {parentOutput.Description}");
                                output.Output.HasHandled();
                            }
                        }

                        if ((!output.Output.Handled && output.Output.NewStatus && !output.HasVirtual) || (output.Output.Handled && output.IsActive))
                        {
                            newValue = ProtocolHelper.SetBit(newValue, output.ModulePin);
                        }

                        if (!output.Output.Handled && !output.HasVirtual)
                        {
                            output.Output.HasHandled();
                        }
                    }

                    // nuovo valore da scrivere sulla memoria del DFCP
                    message.Add(newValue);

                    // calcolo il checksum di 2 bytes
                    message.AddRange(ProtocolHelper.CalculateCHSAt1(message.ToArray()));

                    messages.Add(message.ToArray());
                }
            }

            foreach (var output in outputs.Where(r => r.HasVirtual && !r.Output.Handled))
            {
                // il punto virtuale deve prima essere posto in stato 1 e poi spento manualmente (non è automatico)
                foreach (byte newValue in new byte[] { 1, 0 })
                {
                    List<byte> message = new List<byte>();

                    // indirizzo del nodo DFCP (0 vuol dire leggi su tutti i nodi)
                    message.Add(0);

                    // comando di scrittura punto virtuale
                    message.Add(0x78);

                    // byte totali da cui verrà composta la parte dati della trama
                    message.Add(3);

                    // indirizzo di 2 bytes del punto virtuale su cui scrivere
                    message.AddRange(ProtocolHelper.ConvertToBytes(output.DFCPVirtual, 2));

                    // nuovo stato punto virtuale (acceso / spento)
                    message.Add(newValue);

                    // calcolo il checksum di 2 bytes
                    message.AddRange(ProtocolHelper.CalculateCHSAt1(message.ToArray()));

                    messages.Add(message.ToArray());
                }

                output.Output.HasHandled();
            }

            if (messages.Any())
            {
                using (TcpClient tcpClient = new TcpClient("192.168.1.252", 80))
                {
                    tcpClient.SendTimeout = 500;
                    tcpClient.ReceiveTimeout = 500;

                    using (NetworkStream networkStream = tcpClient.GetStream())
                    {
                        foreach (byte[] message in messages)
                        {
                            byte[] response = new byte[6];
                            await networkStream.WriteAsync(message, 0, message.Length);
                            await networkStream.ReadAsync(response, 0, response.Length);

                            // response[3] contiene la risposta ma attualmente non funziona

                            await Task.Delay(100);
                        }
                    }
                }
            }
        }
    }
}
