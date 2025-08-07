using System;

public interface IGetDataFilter
{
    void GetDataFilter(Func<DataContainer, bool> filter);
}