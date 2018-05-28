
namespace LazyWelfare.AndroidMobile.Loading.Render
{
    using System;
    using Android.Util;
    using LazyWelfare.AndroidMobile.Loading.Render;
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

    public enum LoadingRendererCase
    {
        SwapLoadingRenderer = 4,
        GuardLoadingRenderer = 5,
        DanceLoadingRenderer = 6,
        CollisionLoadingRenderer = 7,
    }

    public sealed class LoadingRendererFactory
    {
        private static readonly SparseArray<Type> LOADING_RENDERERS = new SparseArray<Type>();

        static LoadingRendererFactory()
        {
            //circle rotate
            LOADING_RENDERERS.Put(0, typeof(MaterialLoadingRenderer));
            LOADING_RENDERERS.Put(1, typeof(LevelLoadingRenderer));
            LOADING_RENDERERS.Put(2, typeof(WhorlLoadingRenderer));
            LOADING_RENDERERS.Put(3, typeof(GearLoadingRenderer));
            //circle jump
            LOADING_RENDERERS.Put(4, typeof(SwapLoadingRenderer));
            LOADING_RENDERERS.Put(5, typeof(GuardLoadingRenderer));
            LOADING_RENDERERS.Put(6, typeof(DanceLoadingRenderer));
            LOADING_RENDERERS.Put(7, typeof(CollisionLoadingRenderer));
            //scenery
            LOADING_RENDERERS.Put(8, typeof(DayNightLoadingRenderer));
            LOADING_RENDERERS.Put(9, typeof(ElectricFanLoadingRenderer));
            //animal
            LOADING_RENDERERS.Put(10, typeof(FishLoadingRenderer));
            LOADING_RENDERERS.Put(11, typeof(GhostsEyeLoadingRenderer));
            //goods
            LOADING_RENDERERS.Put(12, typeof(BalloonLoadingRenderer));
            LOADING_RENDERERS.Put(13, typeof(WaterBottleLoadingRenderer));
            //shape change
            LOADING_RENDERERS.Put(14, typeof(CircleBroodLoadingRenderer));
            LOADING_RENDERERS.Put(15, typeof(CoolWaitLoadingRenderer));
        }

        private LoadingRendererFactory()
        {
        }

        public static LoadingRenderer CreateLoadingRenderer(Context context, int loadingRendererId)
        {
            //Type loadingRendererClazz = LOADING_RENDERERS.Get(loadingRendererId);
            //var constructors = loadingRendererClazz.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            //foreach (var constructor in constructors)
            //{
            //    Type[] parameterTypes = constructor.ParameterTypes;
            //    if (parameterTypes != null && parameterTypes.Length == 1 && parameterTypes[0].Equals(typeof(Context)))
            //    {
            //        constructor.Accessible = true;
            //        return (LoadingRenderer)constructor.newInstance(context);
            //    }
            //}

            return new CollisionLoadingRenderer(context);

            throw new InstantiationException();
        }
    }
}