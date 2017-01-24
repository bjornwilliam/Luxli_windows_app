using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luxli_Windows_app
{
	class MasterPacket
	{
		public MasterCommand CommandData { get; protected set; }
		public byte[] Data { get; set; }


		public MasterPacket(MasterCommand command, byte[] data)
		{
			CommandData = command;
			Data = data;
		}
	}
}
