using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace odl;

public interface ILogger
{
	void Write(string message, params object[] args);

	void WriteLine(string message, params object[] args);

	void WriteLine();

	void Warn(string message, params object[] args);

	void Error(string message, params object[] args);
}

public class ConsoleLogger : ILogger
{
	StreamWriter stream;

	public ConsoleLogger()
	{
		stream = new StreamWriter(Console.OpenStandardOutput());
		stream.AutoFlush = true;
	}

	public void Write(string message, params object[] args)
	{
		stream.Write(message, args);
	}

	public void WriteLine(string message, params object[] args)
	{
		stream.WriteLine(message, args);
	}

	public void WriteLine()
	{
		stream.WriteLine();
	}

	public void Warn(string message, params object[] args)
	{
		stream.WriteLine("[WARNING] " + message, args);
	}

	public void Error(string message, params object[] args)
	{
		stream.WriteLine("[ERROR] " + message, args);
	}
}