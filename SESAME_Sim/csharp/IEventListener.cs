using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SESAME_Sim;

public interface IEventListener
{
	string GetID();

	void HandleEvent(EventInstance e);

	void HandleAction(Action a);
}
