
namespace LazyWelfare.AndroidCtrls.LoadingAnimation.Render
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Android.Util;
    using LazyWelfare.AndroidCtrls.LoadingAnimation.Render;
    using System.Reflection;
    using Java.Lang;

    using Context = Android.Content.Context;
    using SparseArray = Android.Util.SparseArray;

    using FishLoadingRenderer = Render.Animal.FishLoadingRenderer;
    using GhostsEyeLoadingRenderer = Render.Animal.GhostsEyeLoadingRenderer;
    using CollisionLoadingRenderer = Render.Circle.Jump.CollisionLoadingRenderer;
    using DanceLoadingRenderer = Render.Circle.Jump.DanceLoadingRenderer;
    using GuardLoadingRenderer = Render.Circle.Jump.GuardLoadingRenderer;
    using SwapLoadingRenderer = Render.Circle.Jump.SwapLoadingRenderer;
    using GearLoadingRenderer = Render.Circle.Rotate.GearLoadingRenderer;
    using LevelLoadingRenderer = Render.Circle.Rotate.LevelLoadingRenderer;
    using MaterialLoadingRenderer = Render.Circle.Rotate.MaterialLoadingRenderer;
    using WhorlLoadingRenderer = Render.Circle.Rotate.WhorlLoadingRenderer;
    using BalloonLoadingRenderer = Render.Goods.BalloonLoadingRenderer;
    using WaterBottleLoadingRenderer = Render.Goods.WaterBottleLoadingRenderer;
    using DayNightLoadingRenderer = Render.Scenery.DayNightLoadingRenderer;
    using ElectricFanLoadingRenderer = Render.Scenery.ElectricFanLoadingRenderer;
    using CircleBroodLoadingRenderer = Render.Shapechange.CircleBroodLoadingRenderer;
    using CoolWaitLoadingRenderer = Render.Shapechange.CoolWaitLoadingRenderer;
    using Android.Graphics;
    using Android.Graphics.Drawables;

    public enum RendererCase
    {
        //circle rotate
        MaterialLoadingRenderer = 0,
        LevelLoadingRenderer = 1,
        WhorlLoadingRenderer = 2,
        GearLoadingRenderer = 3,

        //circle jump
        SwapLoadingRenderer = 4,
        GuardLoadingRenderer = 5,
        DanceLoadingRenderer = 6,
        CollisionLoadingRenderer = 7,

        //scenery
        DayNightLoadingRenderer = 8,
        ElectricFanLoadingRenderer = 9,

        //animal
        FishLoadingRenderer = 10,
        GhostsEyeLoadingRenderer = 11,

        //goods
        BalloonLoadingRenderer = 12,
        WaterBottleLoadingRenderer = 13,

        //shape change
        CircleBroodLoadingRenderer = 14,
        CoolWaitLoadingRenderer = 15,

    }


    public sealed class LoadingRendererFactory
    {
        public static readonly Dictionary<RendererCase, Func<Context, LoadingRenderer>> LoadingRendererDictionary = new Dictionary<RendererCase, Func<Context, LoadingRenderer>>();

        static LoadingRendererFactory()
        {

            //circle rotate
            LoadingRendererDictionary.Add(RendererCase.MaterialLoadingRenderer, (context) => new MaterialLoadingRenderer(context));
            LoadingRendererDictionary.Add(RendererCase.LevelLoadingRenderer, (context) => new LevelLoadingRenderer(context));
            LoadingRendererDictionary.Add(RendererCase.WhorlLoadingRenderer, (context) => new WhorlLoadingRenderer(context));
            LoadingRendererDictionary.Add(RendererCase.GearLoadingRenderer, (context) => new GearLoadingRenderer(context));

            //circle jump
            LoadingRendererDictionary.Add(RendererCase.SwapLoadingRenderer, (context) => new SwapLoadingRenderer(context));
            LoadingRendererDictionary.Add(RendererCase.GuardLoadingRenderer, (context) => new GuardLoadingRenderer(context));
            LoadingRendererDictionary.Add(RendererCase.DanceLoadingRenderer, (context) => new DanceLoadingRenderer(context));
            LoadingRendererDictionary.Add(RendererCase.CollisionLoadingRenderer, (context) => new CollisionLoadingRenderer(context));

            //scenery
            LoadingRendererDictionary.Add(RendererCase.DayNightLoadingRenderer, (context) => new DayNightLoadingRenderer(context));
            LoadingRendererDictionary.Add(RendererCase.ElectricFanLoadingRenderer, (context) => new ElectricFanLoadingRenderer(context));

            //animal
            LoadingRendererDictionary.Add(RendererCase.FishLoadingRenderer, (context) => new FishLoadingRenderer(context));
            LoadingRendererDictionary.Add(RendererCase.GhostsEyeLoadingRenderer, (context) => new GhostsEyeLoadingRenderer(context));

            //goods
            LoadingRendererDictionary.Add(RendererCase.BalloonLoadingRenderer, (context) => new BalloonLoadingRenderer(context));
            LoadingRendererDictionary.Add(RendererCase.WaterBottleLoadingRenderer, (context) => new WaterBottleLoadingRenderer(context));

            //shape change
            LoadingRendererDictionary.Add(RendererCase.CircleBroodLoadingRenderer, (context) => new CircleBroodLoadingRenderer(context));
            LoadingRendererDictionary.Add(RendererCase.CoolWaitLoadingRenderer, (context) => new CoolWaitLoadingRenderer(context));

        }

        public static LoadingRenderer CreateLoadingRenderer(Context context, int loadingRendererId)
        {
            if (System.Enum.IsDefined(typeof(RendererCase), loadingRendererId)==false) throw new InstantiationException($"{nameof(RendererCase)}未定义参数：{ loadingRendererId }");

            var key = (RendererCase)loadingRendererId;

            return CreateLoadingRenderer(context, key);
        }

        public static LoadingRenderer CreateLoadingRenderer(Context context, RendererCase key)
        {

            if (LoadingRendererDictionary.Keys.Contains(key) == false) throw new InstantiationException($" {nameof(LoadingRendererDictionary)} 不包含参数'{ key }'对应的值");

            var creater = LoadingRendererDictionary[key];
            return creater(context);
        }
    }
}