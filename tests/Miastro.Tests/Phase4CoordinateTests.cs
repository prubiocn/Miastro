using Miastro.Domain.Geography;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase4CoordinateTests
{
    [TestMethod]
    [DataRow(-90d)]
    [DataRow(90d)]
    [DataRow(0d)]
    public void Latitude_AcceptsBoundaries(double value)
    {
        Assert.AreEqual(value, new Latitude(value).Value);
    }

    [TestMethod]
    [DataRow(-90.0001)]
    [DataRow(90.0001)]
    public void Latitude_RejectsOutOfRange(double value)
    {
        ArgumentOutOfRangeException? captured = null;

        try
        {
            _ = new Latitude(value);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            captured = ex;
        }

        Assert.IsNotNull(captured);
    }

    [TestMethod]
    [DataRow(-180d)]
    [DataRow(180d)]
    [DataRow(0d)]
    public void Longitude_AcceptsBoundaries(double value)
    {
        Assert.AreEqual(value, new Longitude(value).Value);
    }

    [TestMethod]
    [DataRow(-180.0001)]
    [DataRow(180.0001)]
    public void Longitude_RejectsOutOfRange(double value)
    {
        ArgumentOutOfRangeException? captured = null;

        try
        {
            _ = new Longitude(value);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            captured = ex;
        }

        Assert.IsNotNull(captured);
    }
}
