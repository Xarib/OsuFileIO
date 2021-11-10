using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsuFileIO.HitObject;
using OsuFileIO.HitObject.OsuStd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Tests.OsuFileIO.HitObject;

[TestClass]
public class HitObjectTests
{
    [TestMethod]
    public void Equal_EqualCircles_ReturnsTrue()
    {
        Circle lhs = null;
        Circle rhs = null;
        Assert.IsTrue(lhs == rhs, "Expected to be equal");

        lhs = new Circle(new Coordinates(1, 2), 3);
        rhs = new Circle(new Coordinates(1, 2), 3);

        Assert.IsTrue(lhs == rhs, "Expected to be equal");
    }

    [TestMethod]
    public void NotEqual_UnequalCircles_ReturnsTrue()
    {
        Circle lhs = null;
        Circle rhs = new(new Coordinates(1, 2), 3);
        Assert.IsTrue(lhs != rhs, "Expected to be equal");

        lhs = new(new Coordinates(1, 2), 3);
        rhs = null;
        Assert.IsTrue(lhs != rhs, "Expected to be equal");

        lhs = new(new Coordinates(1, 2), 3);
        rhs = new(new Coordinates(4, 8), 5);
        Assert.IsTrue(lhs != rhs, "Expected to be equal");
    }

    [TestMethod]
    public void Equal_EqualSpinners_ReturnsTrue()
    {
        Spinner lhs = null;
        Spinner rhs = null;
        Assert.IsTrue(lhs == rhs, "Expected to be equal");

        lhs = new(new Coordinates(1, 2), 3, 4);
        rhs = new(new Coordinates(1, 2), 3, 4);

        Assert.IsTrue(lhs == rhs, "Expected to be equal");
    }

    [TestMethod]
    public void NotEqual_UnequalSpinners_ReturnsTrue()
    {
        Spinner lhs = null;
        Spinner rhs = new(new Coordinates(1, 2), 3, 4);
        Assert.IsTrue(lhs != rhs, "Expected to be equal");

        lhs = new(new Coordinates(1, 2), 3, 4);
        rhs = null;
        Assert.IsTrue(lhs != rhs, "Expected to be equal");

        lhs = new(new Coordinates(1, 2), 3, 4);
        rhs = new(new Coordinates(4, 8), 5, 39);
        Assert.IsTrue(lhs != rhs, "Expected to be equal");
    }

    [TestMethod]
    public void Equal_EqualSlider_ReturnsTrue()
    {
        Slider lhs = null;
        Slider rhs = null;
        Assert.IsTrue(lhs == rhs, "Expected to be equal");

        rhs = new(new Coordinates(1, 2), 3, new List<Coordinates> { new Coordinates(1, 2), new Coordinates(4, 6) }, 4.4, CurveType.Bézier, 3);
        lhs = new(new Coordinates(1, 2), 3, new List<Coordinates> { new Coordinates(1, 2), new Coordinates(4, 6) }, 4.4, CurveType.Bézier, 3);

        Assert.IsTrue(lhs == rhs, "Expected to be equal");
    }

    [TestMethod]
    public void NotEqual_UnequalSliders_ReturnsTrue()
    {
        Slider lhs = null;
        Slider rhs = new(new Coordinates(1, 2), 3, new List<Coordinates> { new Coordinates(1, 2), new Coordinates(4, 6) }, 4.4, CurveType.Bézier, 3);
        Assert.IsTrue(lhs != rhs, "Expected to be unequal");

        lhs = new(new Coordinates(1, 2), 3, new List<Coordinates> { new Coordinates(1, 2), new Coordinates(4, 6) }, 4.4, CurveType.Bézier, 3);
        rhs = null;
        Assert.IsTrue(lhs != rhs, "Expected to be unequal");


        rhs = new(new Coordinates(11, 22), 33, new List<Coordinates> { new Coordinates(11, 22), new Coordinates(33, 65) }, 4.6, CurveType.Linear, 4);
        Assert.IsTrue(lhs != rhs, "Expected to be unequal");
    }
}
