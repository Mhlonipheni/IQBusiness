using IQBusiness.RabbitMQ.Shared;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;

namespace IQBusiness.NameSenderApp
{
	class Program
	{
		static void Main(string[] args)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
			IConfigurationRoot configuration = builder.Build();
			Console.WriteLine("Name");
			string myName = Console.ReadLine();
			string nameMessage = $"Hello my name is,{myName}";
			sendNameMessage(nameMessage, configuration);
			Console.ReadLine();
		}
		private static void sendNameMessage(string nameMessage, IConfiguration configuration)
		{
			var hostName = configuration.GetSection("RabbitMQConnectionString")["HostName"];
			var userName = configuration.GetSection("RabbitMQConnectionString")["UserName"];
			var password = configuration.GetSection("RabbitMQConnectionString")["Password"];
			var messageQueueName = configuration.GetSection("QueueDetails")["Queue"];
			RabbitMQService rabbitMQService = new RabbitMQService(hostName, userName, password);
			rabbitMQService.MessageQueueName = messageQueueName;
			try
			{
				var connection = rabbitMQService.GetConnection();
				var channel = connection.CreateModel();
				channel.QueueDeclare(queue: rabbitMQService.MessageQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
				var body = Encoding.UTF8.GetBytes(nameMessage);
				channel.BasicPublish(exchange: "", routingKey: rabbitMQService.MessageQueueName, mandatory: true, basicProperties: null, body: body);
				Console.WriteLine("message published");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}
