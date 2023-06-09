﻿using RabbitMQ.Client;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using DSED_M07_Entites;
using System.Text;
using DSED_M07_TraitementCommande_facturation;
using System.IO;
using System.Runtime.CompilerServices;

public class Program
{
    public static void Main(string[] args)
    {
        string requetesSujet = "commande.placee.";

        ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
        using (IConnection connection = factory.CreateConnection())
        {
            using (IModel channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "m07-commandes",
                                        type: ExchangeType.Topic,
                                        durable: true,
                                        autoDelete: false);

                channel.QueueDeclare(queue: "m07-facturation",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.QueueBind(queue: "m07-facturation",
                                  exchange: "m07-commandes",
                                  routingKey: requetesSujet);

                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    byte[] body = ea.Body.ToArray();
                    string json = Encoding.UTF8.GetString(body);
                    Enveloppe enveloppe = JsonConvert.DeserializeObject<Enveloppe>(json);
                    Commande commande = enveloppe.Commande;

                    switch (enveloppe.Type)
                    {
                        case TypeEnveloppe.normal:
                            Facture factureNormal = new Facture(commande, 0);
                            CreerFichierFacture(factureNormal);
                            break;

                        case TypeEnveloppe.premium:
                            Facture facturePremium = new Facture(commande, 5);
                            CreerFichierFacture(facturePremium);
                            break;

                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Erreur lors du traitement d'une enveloppe, celle-ci n'a pas été traité.");
                            Console.ForegroundColor= ConsoleColor.White;
                            break;
                    }
                };

                channel.BasicConsume(queue: "m07-facturation",
                        autoAck: true,
                        consumer: consumer);

                Console.In.ReadLine();
            }
        } 
    }

    private static void CreerFichierFacture(Facture p_facture)
    {
        if (!Directory.Exists("Factures"))
        {
            Directory.CreateDirectory("Factures");
        }

        DateTime dateActuelle = DateTime.Now;
        string nomFichier = $"{dateActuelle.Year}{dateActuelle.Month}{dateActuelle.Day}_{dateActuelle.Hour}{dateActuelle.Minute}{dateActuelle.Second}_{p_facture.ReferenceCommande}_Facture.json";

        File.WriteAllText($"Factures/{nomFichier}", JsonConvert.SerializeObject(p_facture, Formatting.Indented));
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Fichier Créé ! ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"Path: {nomFichier}");
    }
}