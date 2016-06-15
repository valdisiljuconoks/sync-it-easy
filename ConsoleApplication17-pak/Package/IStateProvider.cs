using System.Collections.Generic;

namespace ConsoleApplication17_pak.Package
{
    public interface IStateProvider
    {
        IEnumerable<IState> GetStates();

    }
}