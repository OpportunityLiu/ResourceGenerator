using System;

namespace Windows.ApplicationModel.Resources
{
    //
    // 摘要:
    //     提供对应用程序资源的简化访问，例如应用程序 UI 字符串。
    public sealed class ResourceLoader
    {
        //
        // 摘要:
        //     为指定的 ResourceMap 构造新的 ResourceLoader 对象。
        //
        // 参数:
        //   name:
        //     新的资源加载程序用于非限定资源引用的 ResourceMap 的资源标识符。 该标识符随后可检索与这些引用相关的资源。该资源标识符被视为统一资源标识符 (URI)
        //     片段，受统一资源标识符 (URI) 语义约束。 例如，Caption%20 被视为 Caption。 不使用？ 或资源标识符中的 #，因为它们会终止已命名的资源路径。
        //     例如，Foo?3 被视为 Foo 处理。
        [Obsolete("ResourceLoader may be altered or unavailable for releases after Windows 8.1. Instead, use GetForCurrentView.")]
        public ResourceLoader(string name)
        {
            this.name = name;
        }

        private string name;

        //
        // 摘要:
        //     为当前正在运行应用的主 ResourceMap 的 Resources 子树构造新的 ResourceLoader 对象。
        public ResourceLoader()
        {
            this.name = "/";
        }

        //
        // 摘要:
        //     返回资源最合适的字符串值，其由资源标识符指定，可用于通过使用 ResourceLoader.GetForCurrentView 而获取的 ResourceLoader
        //     的所在视图的默认 ResourceContext。
        //
        // 参数:
        //   resource:
        //     要解析的资源的资源标识符。该资源标识符被视为统一资源标识符 (URI) 片段，受统一资源标识符 (URI) 语义约束。 例如，getString(Caption%20)
        //     被视为 getString(Caption )。 不使用？ 或资源标识符中的 #，因为它们会终止已命名的资源路径。 例如，Foo?3 被视为 Foo 处理。
        //
        // 返回结果:
        //     默认 ResourceContext 的指定资源最合适的字符串值。
        public string GetString(string resource)
        {
            return $"{name}/{resource}";
        }
        public string GetStringForUri(Uri uri)
        {
            return null;
        }
        //
        // 摘要:
        //     为当前正在运行应用的主 ResourceMap 的 Resources 子树获取 ResourceLoader 对象。
        //
        // 返回结果:
        //     一个资源加载程序，它针对的是当前正在运行应用的主 ResourceMap 的 Resources 子树。
        public static ResourceLoader GetForCurrentView()
        {
            return new ResourceLoader();
        }
        //
        // 摘要:
        //     获取指定的 ResourceMap 的 ResourceLoader 对象。
        //
        // 参数:
        //   name:
        //     新的资源加载程序用于非限定资源引用的 ResourceMap 的资源标识符。 该加载程序随后可检索与这些引用相关的资源。该资源标识符被视为统一资源标识符
        //     (URI) 片段，受统一资源标识符 (URI) 语义约束。 例如，Caption%20 被视为 Caption。 不使用？ 或资源标识符中的 #，因为它们会终止已命名的资源路径。
        //     例如，Foo?3 被视为 Foo 处理。
        //
        // 返回结果:
        //     指定的 ResourceMap 的资源加载程序。
        public static ResourceLoader GetForCurrentView(string name)
        {
            return new ResourceLoader { name = name };
        }
        //
        // 摘要:
        //     为当前正在运行应用的主 ResourceMap 的 Resources 子树获取 ResourceLoader 对象。 该 ResourceLoader
        //     使用不与任何视图关联的默认上下文。
        //
        // 返回结果:
        //     一个资源加载程序，它针对的是当前正在运行应用的主 ResourceMap 的 Resources 子树。 该 ResourceLoader 使用不与任何视图关联的默认上下文。
        //     无法使用此 ResourceLoader 检索任何具有缩放限定的资源候选的资源。
        public static ResourceLoader GetForViewIndependentUse()
        {
            return new ResourceLoader();
        }
        //
        // 摘要:
        //     获取指定的 ResourceMap 的 ResourceLoader 对象。 该 ResourceLoader 使用不与任何视图关联的默认上下文。
        //
        // 参数:
        //   name:
        //     新的资源加载程序用于非限定资源引用的 ResourceMap 的资源标识符。 该加载程序随后可检索与这些引用相关的资源。该资源标识符被视为统一资源标识符
        //     (URI) 片段，受统一资源标识符 (URI) 语义约束。 例如，Caption%20 被视为 Caption。 不使用？ 或资源标识符中的 #，因为它们会终止已命名的资源路径。
        //     例如，Foo?3 被视为 Foo 处理。
        //
        // 返回结果:
        //     一个资源加载程序，它针对的是当前正在运行应用的主 ResourceMap 的 Resources 子树。 该 ResourceLoader 使用不与任何视图关联的默认上下文。
        //     无法使用此 ResourceLoader 检索任何具有缩放限定的资源候选的资源。
        public static ResourceLoader GetForViewIndependentUse(string name)
        {
            return new ResourceLoader { name = name };
        }

        [Obsolete("GetStringForReference may be altered or unavailable for releases after Windows Phone 'OSVersion' (TBD). Instead, use GetStringForUri.")]
        public static string GetStringForReference(Uri uri)
        {
            return new ResourceLoader().GetStringForUri(uri);
        }
    }
}