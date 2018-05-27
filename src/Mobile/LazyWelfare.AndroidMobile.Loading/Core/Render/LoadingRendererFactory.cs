
namespace LazyWelfare.AndroidMobile.Loading.Render
{
    using System;
    using Android.Util;
    using LazyWelfare.AndroidMobile.Loading.Render;
    using System.Reflection;

    using Context = Android.Content.Context;
    using SparseArray = Android.Util.SparseArray;

    //using FishLoadingRenderer = Render.animal.FishLoadingRenderer;
    //using GhostsEyeLoadingRenderer = Render.animal.GhostsEyeLoadingRenderer;
    using CollisionLoadingRenderer = Render.Circle.Jump.CollisionLoadingRenderer;
    using DanceLoadingRenderer = Render.Circle.Jump.DanceLoadingRenderer;
    using GuardLoadingRenderer = Render.Circle.Jump.GuardLoadingRenderer;
    using SwapLoadingRenderer = Render.Circle.Jump.SwapLoadingRenderer;
    using Java.Lang;

    //using GearLoadingRenderer = Render.circle.rotate.GearLoadingRenderer;
    //using LevelLoadingRenderer = Render.circle.rotate.LevelLoadingRenderer;
    //using MaterialLoadingRenderer = Render.circle.rotate.MaterialLoadingRenderer;
    //using WhorlLoadingRenderer = Render.circle.rotate.WhorlLoadingRenderer;
    //using BalloonLoadingRenderer = Render.goods.BalloonLoadingRenderer;
    //using WaterBottleLoadingRenderer = Render.goods.WaterBottleLoadingRenderer;
    //using DayNightLoadingRenderer = Render.scenery.DayNightLoadingRenderer;
    //using ElectricFanLoadingRenderer = Render.scenery.ElectricFanLoadingRenderer;
    //using CircleBroodLoadingRenderer = Render.shapechange.CircleBroodLoadingRenderer;
    //using CoolWaitLoadingRenderer = Render.shapechange.CoolWaitLoadingRenderer;

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
            ////circle rotate
            //LOADING_RENDERERS.Put(0, typeof(MaterialLoadingRenderer));
            //LOADING_RENDERERS.Put(1, typeof(LevelLoadingRenderer));
            //LOADING_RENDERERS.Put(2, typeof(WhorlLoadingRenderer));
            //LOADING_RENDERERS.Put(3, typeof(GearLoadingRenderer));
            //circle jump
            LOADING_RENDERERS.Put(4, typeof(SwapLoadingRenderer));
            LOADING_RENDERERS.Put(5, typeof(GuardLoadingRenderer));
            LOADING_RENDERERS.Put(6, typeof(DanceLoadingRenderer));
            LOADING_RENDERERS.Put(7, typeof(CollisionLoadingRenderer));
            ////scenery
            //LOADING_RENDERERS.Put(8, typeof(DayNightLoadingRenderer));
            //LOADING_RENDERERS.Put(9, typeof(ElectricFanLoadingRenderer));
            ////animal
            //LOADING_RENDERERS.Put(10, typeof(FishLoadingRenderer));
            //LOADING_RENDERERS.Put(11, typeof(GhostsEyeLoadingRenderer));
            ////goods
            //LOADING_RENDERERS.Put(12, typeof(BalloonLoadingRenderer));
            //LOADING_RENDERERS.Put(13, typeof(WaterBottleLoadingRenderer));
            ////shape change
            //LOADING_RENDERERS.Put(14, typeof(CircleBroodLoadingRenderer));
            //LOADING_RENDERERS.Put(15, typeof(CoolWaitLoadingRenderer));
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

            return new SwapLoadingRenderer(context);

            throw new InstantiationException();
        }
    }
}