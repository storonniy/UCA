using UCA.DeviceDrivers;

namespace Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void GetRelaysAsByteMoreTests()
    {
        GetRelaysAsByteTest(new int[] {0, 1, 2, 3, 4, 5, 6, 7}, 255);
        GetRelaysAsByteTest(new int[] {0, 7}, 129);
    }

    private void GetRelaysAsByteTest(IEnumerable<int> relayNumbers, byte expected)
    {
        var actual = PCI_1762.ConvertRelayNumbersToByte(relayNumbers);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetPortNumDictionaryTests()
    {
        GetPortNumDictionaryTest(new int[] {0, 1, 2, 8, 9, 10, 15},
            new Dictionary<int, byte>() {{0, 7}, {1, 135}});
    }

    [Test]
    public void OneRelayInOnePort()
    {
        GetPortNumDictionaryTest(new int[] {0},
            new Dictionary<int, byte>() {{0, 1}});
        GetPortNumDictionaryTest(new int[] {5},
            new Dictionary<int, byte>() {{0, 32}});
        GetPortNumDictionaryTest(new int[] {8},
            new Dictionary<int, byte>() {{1, 1}});
        GetPortNumDictionaryTest(new int[] {13},
            new Dictionary<int, byte>() {{1, 32}});
    }

    private void GetPortNumDictionaryTest(IEnumerable<int> relayNumbers, Dictionary<int, byte> expected)
    {
        var actual = PCI_1762.GetPortBytesDictionary(relayNumbers);
        CollectionAssert.AreEquivalent(expected, actual);
    }
}