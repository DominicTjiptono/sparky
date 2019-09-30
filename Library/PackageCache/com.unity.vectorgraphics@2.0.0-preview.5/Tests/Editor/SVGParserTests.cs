﻿using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;

public class SVGParserTests
{
    [Test]
    public void ImportSVG_CreatesScene()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""100"" height=""20"">
                <rect width=""100"" height=""20"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        Assert.IsNotNull(sceneInfo.Scene);
        Assert.IsNotNull(sceneInfo.Scene.Root);
    }

    [Test]
    public void ImportSVG_SupportsRects()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""100"" height=""20"">
                <rect x=""5"" y=""10"" width=""100"" height=""20"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var rectShape = sceneInfo.Scene.Root.Children[0].Shapes[0];
        Assert.IsNotNull(rectShape);
        Assert.IsTrue(rectShape.IsConvex);
        Assert.AreEqual(1, rectShape.Contours.Length);

        var contour = rectShape.Contours[0];
        Assert.AreEqual(4, contour.Segments.Length);
        Assert.AreEqual(new Vector2(5.0f, 30.0f), contour.Segments[0].P0);
        Assert.AreEqual(new Vector2(5.0f, 10.0f), contour.Segments[1].P0);
        Assert.AreEqual(new Vector2(105.0f, 10.0f), contour.Segments[2].P0);
        Assert.AreEqual(new Vector2(105.0f, 30.0f), contour.Segments[3].P0);
    }

    [Test]
    public void ImportSVG_SupportsCircles()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""100"" height=""20"">
                <circle cx=""50"" cy=""60"" r=""50"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var circleShape = sceneInfo.Scene.Root.Children[0].Shapes[0];
        Assert.IsNotNull(circleShape);
        Assert.IsTrue(circleShape.IsConvex);
        Assert.AreEqual(1, circleShape.Contours.Length);

        var contour = circleShape.Contours[0];
        Assert.AreEqual(4, contour.Segments.Length);
        Assert.AreEqual(100.0f, (contour.Segments[2].P0-contour.Segments[0].P0).magnitude);
        Assert.AreEqual(100.0f, (contour.Segments[3].P0-contour.Segments[1].P0).magnitude);
    }

    [Test]
    public void ImportSVG_SupportsEllipses()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""100"" height=""20"">
                <ellipse cx=""50"" cy=""60"" rx=""50"" ry=""60"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var ellipseShape = sceneInfo.Scene.Root.Children[0].Shapes[0];
        Assert.IsNotNull(ellipseShape);
        Assert.IsTrue(ellipseShape.IsConvex);
        Assert.AreEqual(1, ellipseShape.Contours.Length);

        var contour = ellipseShape.Contours[0];
        Assert.AreEqual(4, contour.Segments.Length);
        Assert.AreEqual(100.0f, (contour.Segments[2].P0 - contour.Segments[0].P0).magnitude);
        Assert.AreEqual(120.0f, (contour.Segments[3].P0 - contour.Segments[1].P0).magnitude);
    }

    [Test]
    public void ImportSVG_SupportsPaths()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""100"" height=""20"">
                <path d=""M10,10L100,100"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));

        var shape = sceneInfo.Scene.Root.Children[0].Shapes[0];
        Assert.IsNotNull(shape);
        Assert.AreEqual(1, shape.Contours.Length);
        
        var segs = shape.Contours[0].Segments;
        Assert.AreEqual(2, segs.Length);
        Assert.AreEqual(new Vector2(10, 10), segs[0].P0);
        Assert.AreEqual(new Vector2(100, 100), segs[1].P0);
    }

    [Test]
    public void ImportSVG_SupportsLines()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""100"" height=""20"">
                <line x1=""10"" y1=""10"" x2=""100"" y2=""100"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));

        var path = sceneInfo.Scene.Root.Children[0].Shapes[0];
        Assert.IsNotNull(path);
        Assert.IsNotNull(path.Contours);
        Assert.AreEqual(1, path.Contours.Length);

        var segs = path.Contours[0].Segments;
        Assert.AreEqual(2, segs.Length);
        Assert.AreEqual(new Vector2(10, 10), segs[0].P0);
        Assert.AreEqual(new Vector2(100, 100), segs[1].P0);
    }

    [Test]
    public void ImportSVG_SupportsPolylines()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""100"" height=""20"">
                <polyline points=""10,10,100,100"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));

        var shape = sceneInfo.Scene.Root.Children[0].Shapes[0];
        Assert.IsNotNull(shape);
        Assert.AreEqual(1, shape.Contours.Length);

        var segs = shape.Contours[0].Segments;
        Assert.AreEqual(2, segs.Length);
        Assert.AreEqual(new Vector2(10, 10), segs[0].P0);
        Assert.AreEqual(new Vector2(100, 100), segs[1].P0);
    }

    [Test]
    public void ImportSVG_SupportsPolygons()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""100"" height=""20"">
                <polygon points=""10,10,100,100"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));

        var shape = sceneInfo.Scene.Root.Children[0].Shapes[0];
        Assert.IsNotNull(shape);
        Assert.AreEqual(1, shape.Contours.Length);

        var segs = shape.Contours[0].Segments;
        Assert.AreEqual(2, segs.Length);
        Assert.AreEqual(new Vector2(10, 10), segs[0].P0);
        Assert.AreEqual(new Vector2(100, 100), segs[1].P0);
    }

    [Test]
    public void ImportSVG_SupportsSolidFills()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""100"" height=""20"">
                <rect x=""5"" y=""10"" width=""100"" height=""20"" fill=""red"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.Scene.Root.Children[0].Shapes[0];
        var fill = shape.Fill as SolidFill;
        Assert.AreEqual(Color.red, fill.Color);
    }

    [Test]
    public void ImportSVG_SupportsLinearGradientFills()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""100"" height=""20"">
                <defs>
                    <linearGradient id=""grad"">
                        <stop offset=""0%"" stop-color=""blue"" />
                        <stop offset=""100%"" stop-color=""red"" />
                    </linearGradient>
                </defs>
                <rect x=""5"" y=""10"" width=""100"" height=""20"" fill=""url(#grad)"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.Scene.Root.Children[0].Shapes[0];
        var fill = shape.Fill as GradientFill;
        Assert.AreEqual(GradientFillType.Linear, fill.Type);
        Assert.AreEqual(2, fill.Stops.Length);
        Assert.AreEqual(0.0f, fill.Stops[0].StopPercentage);
        Assert.AreEqual(Color.blue, fill.Stops[0].Color);
        Assert.AreEqual(1.0f, fill.Stops[1].StopPercentage);
        Assert.AreEqual(Color.red, fill.Stops[1].Color);
    }

    [Test]
    public void ImportSVG_SupportsRadialGradientFills()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""100"" height=""20"">
                <defs>
                    <radialGradient id=""grad"">
                        <stop offset=""0%"" stop-color=""blue"" />
                        <stop offset=""100%"" stop-color=""red"" />
                    </radialGradient>
                </defs>
                <rect x=""5"" y=""10"" width=""100"" height=""20"" fill=""url(#grad)"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.Scene.Root.Children[0].Shapes[0];
        var fill = shape.Fill as GradientFill;
        Assert.AreEqual(GradientFillType.Radial, fill.Type);
        Assert.AreEqual(2, fill.Stops.Length);
        Assert.AreEqual(0.0f, fill.Stops[0].StopPercentage);
        Assert.AreEqual(Color.blue, fill.Stops[0].Color);
        Assert.AreEqual(1.0f, fill.Stops[1].StopPercentage);
        Assert.AreEqual(Color.red, fill.Stops[1].Color);
    }

    [Test]
    public void ImportSVG_SupportsFillOpacities()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""100"" height=""20"">
                <rect x=""5"" y=""10"" width=""100"" height=""20"" fill=""red"" fill-opacity=""0.5"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.Scene.Root.Children[0].Shapes[0];
        var fill = shape.Fill as SolidFill;
        Assert.AreEqual(0.5f, fill.Color.a);
    }

    [Test]
    public void ImportSVG_DefaultFillIsBlack()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""100"" height=""100"">
                <rect x=""0"" y=""0"" width=""100"" height=""100"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.Scene.Root.Children[0].Shapes[0];
        Assert.IsNotNull(shape.Fill);
        var fill = shape.Fill as SolidFill;
        Assert.AreEqual(Color.black, fill.Color);
    }

    [Test]
    public void ImportSVG_NoneFillIsNull()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""100"" height=""100"">
                <rect x=""0"" y=""0"" width=""100"" height=""100"" fill=""none"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.Scene.Root.Children[0].Shapes[0];
        Assert.IsNull(shape.Fill);
    }

    [Test]
    public void ImportSVG_NoneStrokeIsNull()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""100"" height=""100"">
                <rect x=""0"" y=""0"" width=""100"" height=""100"" stroke=""none"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.Scene.Root.Children[0].Shapes[0];
        Assert.IsNull(shape.PathProps.Stroke);
    }

    [Test]
    public void ImportSVG_SupportsGroups()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""100"" height=""20"">
                <g>
                    <rect x=""5"" y=""10"" width=""100"" height=""20"" />
                    <rect x=""5"" y=""50"" width=""100"" height=""20"" />
                </g>
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        Assert.AreEqual(1, sceneInfo.Scene.Root.Children.Count);

        var group = sceneInfo.Scene.Root.Children[0];
        Assert.AreEqual(2, group.Children.Count);
    }

    [Test]
    public void ImportSVG_RemovesElementsWithDisplayNone()
    {
        string svg = 
            @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""100"" height=""20"">
                <rect width=""100"" height=""20"" display=""none"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        Assert.AreEqual(0, sceneInfo.Scene.Root.Children.Count);
    }

    [Test]
    public void PreserveViewport_ClipsInViewportSpace()
    {
        string svg =
           @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""100"" height=""100"" viewBox=""600 600 100 100"">
                <rect x=""590"" y=""590"" width=""100"" height=""100"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg), 0.0f, 1.0f, 0, 0, true);
        var options = new VectorUtils.TessellationOptions()
        {
            StepDistance = 100.0f,
            MaxCordDeviation = float.MaxValue,
            MaxTanAngleDeviation = float.MaxValue,
            SamplingStepSize = 0.01f
        };
        var geom = VectorUtils.TessellateScene(sceneInfo.Scene, options);
        Assert.That(geom.Count == 1);
        Assert.That(geom[0].Vertices.Length > 0);
    }

    [Test]
    public void ImportSVG_UseHaveFillAndStrokeFromSymbol()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" width=""100"" height=""100"">
                <defs>
                    <rect id=""Rect"" width=""10"" height=""10"" fill=""#FF0000"" stroke=""#00FF00"" />
                </defs>
                <use id=""Rect0"" xlink:href=""#Rect"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.NodeIDs["Rect0"].Shapes[0];
        var fill = shape.Fill as SolidFill;
        var stroke = shape.PathProps.Stroke;

        Assert.AreEqual(Color.red, fill.Color);
        Assert.AreEqual(Color.green, stroke.Color);
    }

    [Test]
    public void ImportSVG_UseHaveDefaultFillFromSymbol()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" width=""100"" height=""100"">
                <defs>
                    <rect id=""Rect"" width=""10"" height=""10"" />
                </defs>
                <use id=""Rect0"" xlink:href=""#Rect"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.NodeIDs["Rect0"].Shapes[0];
        var fill = shape.Fill as SolidFill;

        Assert.AreEqual(Color.black, fill.Color);
    }

    [Test]
    public void ImportSVG_UseCanOverrideSymbolFillAndStroke()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" width=""100"" height=""100"">
                <defs>
                    <rect id=""Rect"" width=""10"" height=""10"" />
                </defs>
                <use id=""Rect0"" xlink:href=""#Rect"" />
                <use id=""Rect1"" xlink:href=""#Rect"" fill=""#0000FF"" />
                <use id=""Rect2"" xlink:href=""#Rect"" stroke=""#0000FF"" />
                <use id=""Rect3"" xlink:href=""#Rect"" fill=""#0000FF"" stroke=""#0000FF"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape0 = sceneInfo.NodeIDs["Rect0"].Shapes[0];
        var shape1 = sceneInfo.NodeIDs["Rect1"].Shapes[0];
        var shape2 = sceneInfo.NodeIDs["Rect2"].Shapes[0];
        var shape3 = sceneInfo.NodeIDs["Rect3"].Shapes[0];

        var fill0 = shape0.Fill as SolidFill;
        var fill1 = shape1.Fill as SolidFill;
        var fill2 = shape2.Fill as SolidFill;
        var fill3 = shape3.Fill as SolidFill;

        var stroke0 = shape0.PathProps.Stroke;
        var stroke1 = shape1.PathProps.Stroke;
        var stroke2 = shape2.PathProps.Stroke;
        var stroke3 = shape3.PathProps.Stroke;

        Assert.AreEqual(Color.black, fill0.Color);
        Assert.AreEqual(Color.blue, fill1.Color);
        Assert.AreEqual(Color.black, fill2.Color);
        Assert.AreEqual(Color.blue, fill3.Color);

        Assert.IsNull(stroke0);
        Assert.IsNull(stroke1);
        Assert.AreEqual(Color.blue, stroke2.Color);
        Assert.AreEqual(Color.blue, stroke3.Color);
    }

    [Test]
    public void ImportSVG_UseCanOverrideSymbolWithEmptyFillWithColor()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" width=""100"" height=""100"">
                <defs>
                    <rect id=""Rect"" width=""10"" height=""10"" />
                </defs>
                <use id=""Rect"" xlink:href=""#Rect"" fill=""black"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.NodeIDs["Rect"].Shapes[0];
        var solidFill = shape.Fill as SolidFill;
        Assert.AreEqual(Color.black, solidFill.Color);
    }

    [Test]
    public void ImportSVG_UseCanOverrideSymbolWithEmptyFillWithNone()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" width=""100"" height=""100"">
                <defs>
                    <rect id=""Rect"" width=""10"" height=""10"" />
                </defs>
                <use id=""Rect"" xlink:href=""#Rect"" fill=""none"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.NodeIDs["Rect"].Shapes[0];
        Assert.IsNull(shape.Fill);
    }

    public void ImportSVG_UseCannotOverrideFillOnSymbolWitNoneFill()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" width=""100"" height=""100"">
                <defs>
                    <rect id=""Rect"" width=""10"" height=""10"" fill=""none"" />
                </defs>
                <use id=""Rect"" xlink:href=""#Rect"" fill=""red"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.NodeIDs["Rect"].Shapes[0];
        Assert.IsNull(shape.Fill);            
    }

    public void ImportSVG_UseCannotOverrideFillOnSymbolWithBlackFill()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" width=""100"" height=""100"">
                <defs>
                    <rect id=""Rect"" width=""10"" height=""10"" fill=""black"" />
                </defs>
                <use id=""Rect"" xlink:href=""#Rect"" fill=""red"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.NodeIDs["Rect"].Shapes[0];
        var solidFill = shape.Fill as SolidFill;
        Assert.IsNotNull(solidFill);
        Assert.AreEqual(Color.black, solidFill.Color);
    }

    [Test]
    public void ImportSVG_UseCanOverrideFillWithTransparentFill()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" width=""100"" height=""100"">
                <defs>
                    <rect id=""Rect"" width=""10"" height=""10"" />
                </defs>
                <use id=""Rect"" xlink:href=""#Rect"" fill-opacity=""0"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.NodeIDs["Rect"].Shapes[0];
        var solidFill = shape.Fill as SolidFill;
        Assert.IsNotNull(solidFill);
        Assert.AreEqual(new Color(0,0,0,0), solidFill.Color);
    }

    [Test]
    public void ImportSVG_UseCanOverrideSymbolStroke()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" width=""100"" height=""100"">
                <defs>
                    <rect id=""Rect"" width=""10"" height=""10"" fill=""red"" />
                </defs>
                <use id=""Rect"" xlink:href=""#Rect"" stroke=""black"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.NodeIDs["Rect"].Shapes[0];
        Assert.IsNotNull(shape.PathProps.Stroke);
        Assert.AreEqual(Color.black, shape.PathProps.Stroke.Color);
    }

    [Test]
    public void ImportSVG_UseCannotOverrideSymbolStrokeWhenDefined()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" width=""100"" height=""100"">
                <defs>
                    <rect id=""Rect"" width=""10"" height=""10"" fill=""red"" stroke=""black"" />
                </defs>
                <use id=""Rect"" xlink:href=""#Rect"" stroke=""red"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.NodeIDs["Rect"].Shapes[0];
        Assert.IsNotNull(shape.PathProps.Stroke);
        Assert.AreEqual(Color.black, shape.PathProps.Stroke.Color);
    }

    [Test]
    public void ImportSVG_UseCanOverrideSymbolStrokeWidth()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" width=""100"" height=""100"">
                <defs>
                    <rect id=""Rect"" width=""10"" height=""10"" fill=""red"" stroke=""black"" />
                </defs>
                <use id=""Rect"" xlink:href=""#Rect"" stroke-width=""2"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.NodeIDs["Rect"].Shapes[0];
        Assert.IsNotNull(shape.PathProps.Stroke);
        Assert.AreEqual(Color.black, shape.PathProps.Stroke.Color);
        Assert.AreEqual(1.0f, shape.PathProps.Stroke.HalfThickness);
    }

    [Test]
    public void ImportSVG_UseCannotOverrideSymbolStrokeWidthWhenDefined()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" width=""100"" height=""100"">
                <defs>
                    <rect id=""Rect"" width=""10"" height=""10"" fill=""red"" stroke=""black"" stroke-width=""2"" />
                </defs>
                <use id=""Rect"" xlink:href=""#Rect"" stroke-width=""1"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.NodeIDs["Rect"].Shapes[0];
        Assert.IsNotNull(shape.PathProps.Stroke);
        Assert.AreEqual(Color.black, shape.PathProps.Stroke.Color);
        Assert.AreEqual(1.0f, shape.PathProps.Stroke.HalfThickness);
    }

    [Test]
    public void ImportSVG_UseCanReferenceElementDefinedLater()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" width=""100"" height=""100"">
                <use id=""Rect"" xlink:href=""#Symbol"" stroke=""black"" />
                <symbol id=""Symbol"">
                    <rect width=""10"" height=""10"" fill=""red"" />
                </symbol>
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.NodeIDs["Rect"].Children[0].Shapes[0];
        var solidFill = shape.Fill as SolidFill;
        Assert.IsNotNull(solidFill);
        Assert.AreEqual(Color.red, solidFill.Color);
        Assert.IsNotNull(shape.PathProps.Stroke);
        Assert.AreEqual(Color.black, shape.PathProps.Stroke.Color);
    }

    [Test]
    public void ImportSVG_SymbolCanReferenceAnotherSymbolDefinedLater()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" width=""100"" height=""100"">
                <symbol id=""Symbol1"">
                    <use xlink:href=""#Symbol2"" />
                </symbol>
                <symbol id=""Symbol2"">
                    <rect width=""10"" height=""10"" fill=""red"" />
                </symbol>
                <use id=""Rect"" xlink:href=""#Symbol1"" stroke=""black"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.NodeIDs["Rect"].Children[0].Children[0].Children[0].Shapes[0];
        var solidFill = shape.Fill as SolidFill;
        Assert.IsNotNull(solidFill);
        Assert.AreEqual(Color.red, solidFill.Color);
        Assert.IsNotNull(shape.PathProps.Stroke);
        Assert.AreEqual(Color.black, shape.PathProps.Stroke.Color);
    }

    [Test]
    public void ImportSVG_CanReferenceFillsDefinedLater()
    {
        string svg =
            @"<svg width=""54px"" height=""54px"" version=""1.1"" xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" xml:space=""preserve"" xmlns:serif=""http://www.serif.com/"" style=""fill-rule:evenodd;clip-rule:evenodd;stroke-linejoin:round;stroke-miterlimit:1.41421;"">
                <path id=""GradientHill"" d=""M0.012,26.717c0,-14.743 11.97,-26.713 26.714,-26.713c14.743,0 26.713,11.97 26.713,26.713c0,14.743 -11.97,26.713 -26.713,26.713c-14.744,0 -26.714,-11.97 -26.714,-26.713Z"" style=""fill:url(#_Linear1);""/>
                <defs>
                    <linearGradient id=""_Linear1"" x1=""0"" y1=""0"" x2=""1"" y2=""0"" gradientUnits=""userSpaceOnUse"" gradientTransform=""matrix(3.2714e-15,-53.4261,53.4261,3.2714e-15,26.7255,53.43)"">
                    <stop offset=""0"" style=""stop-color:#9d9d9d;stop-opacity:1""/>
                    <stop offset=""0.47"" style=""stop-color:#e9e9e9;stop-opacity:1""/>
                    <stop offset=""1"" style=""stop-color:#fff;stop-opacity:1""/>
                    </linearGradient>
                </defs>
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.NodeIDs["GradientHill"].Shapes[0];
        Assert.IsNotNull(shape.Fill);
        Assert.IsNotNull(shape.Fill as GradientFill);
    }

    [Test, Description("Case 1136667")]
    public void ImportSVG_PolygonSkipsDuplicatedPoints()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" viewBox=""0 0 216 216"">
                <g>
                    <polygon id=""Poly1"" points=""143.781 45.703 72.401 45.703 36.711 107.52 72.401 169.337 143.781 169.337 179.471 107.52 143.781 45.703 143.781 45.703""/>
                </g>
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.NodeIDs["Poly1"].Shapes[0];
        Assert.AreEqual(1, shape.Contours.Length);
        Assert.AreEqual(7, shape.Contours[0].Segments.Length);
    }

    [Test, Description("Jira issue SECURITY-505")]
    public void ImportSVG_ImageDoesNotSupportFileURLScheme()
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" width=""200"" height=""200"">
                <image id=""Image0"" xlink:href=""file://192.168.0.1/image.png"" width=""200"" height=""200"" />
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        SceneNode node;
        Assert.IsFalse(sceneInfo.NodeIDs.TryGetValue("Image0", out node));
    }
}
