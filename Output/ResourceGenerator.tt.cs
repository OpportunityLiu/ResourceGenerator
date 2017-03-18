namespace ResourceGenerator.ResourceGenerator_ResourceInfo
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
    internal interface IResourceProvider
    {
        global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider this[string resourceKey] { get; }
        string GetValue(string resourceKey);
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
    internal interface IGeneratedResourceProvider : global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider
    {
        string Value { get; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
    [System.Diagnostics.DebuggerDisplay("\\{{key,nq}\\}")]
    internal struct GeneratedResourceProvider : global::ResourceGenerator.ResourceGenerator_ResourceInfo.IGeneratedResourceProvider
    {
        internal GeneratedResourceProvider(string key)
        {
            this.key = key;
        }

        private readonly string key;

        public string Value => global::ResourceGenerator.LocalizedStrings.GetValue(key);

        public GeneratedResourceProvider this[string resourceKey]
        {
            get
            {
                if(resourceKey == null)
                    throw new global::System.ArgumentNullException();
                return new global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider($"{key}/{resourceKey}");
            }
        }

        public string GetValue(string resourceKey)
        {
            if(resourceKey == null)
                return this.Value;
            return global::ResourceGenerator.LocalizedStrings.GetValue($"{key}/{resourceKey}");
        }
    }
}

namespace ResourceGenerator.ResourceGenerator_ResourceInfo
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
    internal interface IResource : global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider
    {
        /// <summary>
        /// <para>Eq</para>
        /// </summary>
        string Equals { get; }
        /// <summary>
        /// <para>4</para>
        /// </summary>
        string @int { get; }
        /// <summary>
        /// <para>1</para>
        /// </summary>
        string String1 { get; }
        global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.IString2 String2 { get; }
        /// <summary>
        /// <para>&lt;1&gt;</para>
        /// </summary>
#warning Resource has been renamed. ResourceName: "<11>", PropertyName: "__11_"
        string __11_ { get; }
        /// <summary>
        /// <para>6</para>
        /// </summary>
#warning Resource has been renamed. ResourceName: "ha\"ha", PropertyName: "ha_ha"
        string ha_ha { get; }
#warning Resource has been renamed. ResourceName: "ha.h\"a2", PropertyName: "ha_h_a2"
        global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.Iha_h_a2 ha_h_a2 { get; }
        /// <summary>
        /// <para>	</para>
        /// </summary>
#warning Resource has been renamed. ResourceName: "\t", PropertyName: "__"
        string __ { get; }
        global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.Itest test { get; }
        /// <summary>
        /// <para>Re</para>
        /// </summary>
        new string Reset { get; }
        /// <summary>
        /// <para>MC</para>
        /// </summary>
        string MemberwiseClone { get; }
        /// <summary>
        /// <para>REq</para>
        /// </summary>
        string ReferenceEquals { get; }
        /// <summary>
        /// <para>GT</para>
        /// </summary>
        string GetType { get; }
        /// <summary>
        /// <para>GHC</para>
        /// </summary>
        string GetHashCode { get; }
        new global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.IGetValue GetValue { get; }
        global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.Idouble @double { get; }
    }
}

namespace ResourceGenerator.ResourceGenerator_ResourceInfo.Resource
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
    internal interface IString2 : global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider
    {
        global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.String2.Iaa aa { get; }
    }
}

namespace ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.String2
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
    internal interface Iaa : global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider
    {
        /// <summary>
        /// <para>3</para>
        /// </summary>
        string bb { get; }
    }
}

namespace ResourceGenerator.ResourceGenerator_ResourceInfo.Resource
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
    internal interface Iha_h_a2 : global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider
    {
        /// <summary>
        /// <para>7</para>
        /// </summary>
        string a { get; }
        global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.ha_h_a2.Ib b { get; }
    }
}

namespace ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.ha_h_a2
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
    internal interface Ib : global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider
    {
        /// <summary>
        /// <para>8</para>
        /// </summary>
        string c { get; }
        global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.ha_h_a2.b.Ihaha3 haha3 { get; }
    }
}

namespace ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.ha_h_a2.b
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
    internal interface Ihaha3 : global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider
    {
        /// <summary>
        /// <para>9</para>
        /// </summary>
        string cc { get; }
    }
}

namespace ResourceGenerator.ResourceGenerator_ResourceInfo.Resource
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
    internal interface Itest : global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider
    {
#warning Resource has been renamed. ResourceName: "", PropertyName: "__Empty__"
        global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.test.I__Empty__ __Empty__ { get; }
    }
}

namespace ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.test
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
    internal interface I__Empty__ : global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider
    {
        /// <summary>
        /// </summary>
        string test { get; }
    }
}

namespace ResourceGenerator.ResourceGenerator_ResourceInfo.Resource
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
    internal interface IGetValue : global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider
    {
        /// <summary>
        /// <para>Va</para>
        /// </summary>
        new string GetValue { get; }
    }
}

namespace ResourceGenerator.ResourceGenerator_ResourceInfo.Resource
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
    internal interface Idouble : global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider
    {
        /// <summary>
        /// <para>string of double</para>
        /// </summary>
        string @string { get; }
    }
}

namespace ResourceGenerator
{
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
    internal static class LocalizedStrings
    {
        private static readonly global::System.Collections.Generic.IDictionary<string, string> __cache__mEGbVaRP
            = new global::System.Collections.Generic.Dictionary<string, string>();
        private static readonly global::Windows.ApplicationModel.Resources.ResourceLoader __loader__qEwdJCg_
            = global::Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse();

        internal static string GetValue(string resourceKey)
        {
            string value;
            if(global::ResourceGenerator.LocalizedStrings.__cache__mEGbVaRP.TryGetValue(resourceKey, out value))
                return value;
            return global::ResourceGenerator.LocalizedStrings.__cache__mEGbVaRP[resourceKey] = global::ResourceGenerator.LocalizedStrings.__loader__qEwdJCg_.GetString(resourceKey);
        }


        internal static global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResource Resource { get; } = new global::ResourceGenerator.LocalizedStrings.Resource__6qftPz0F();

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
        [System.Diagnostics.DebuggerDisplay("\\{ms-resource:///Resource\\}")]
        private sealed class Resource__6qftPz0F : global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResource
        {
            global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider.this[string resourceKey]
            {
                get
                {
                    if(resourceKey == null)
                        throw new global::System.ArgumentNullException();
                    return new global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider("ms-resource:///Resource/" + resourceKey);
                }
            }

            string global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider.GetValue(string resourceKey)
            {
                return global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/" + resourceKey);
            }


            global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.IString2 global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResource.String2 { get; } = new global::ResourceGenerator.LocalizedStrings.Resource__6qftPz0F.String2__53sKdMIZ();

            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
            [System.Diagnostics.DebuggerDisplay("\\{ms-resource:///Resource/String2\\}")]
            private sealed class String2__53sKdMIZ : global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.IString2
            {
                global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider.this[string resourceKey]
                {
                    get
                    {
                        if(resourceKey == null)
                            throw new global::System.ArgumentNullException();
                        return new global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider("ms-resource:///Resource/String2/" + resourceKey);
                    }
                }

                string global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider.GetValue(string resourceKey)
                {
                    if(resourceKey == null)
                        return global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/String2");
                    return global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/String2/" + resourceKey);
                }

                global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.String2.Iaa global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.IString2.aa { get; } = new global::ResourceGenerator.LocalizedStrings.Resource__6qftPz0F.String2__53sKdMIZ.aa__hzfn5FNO();

                [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
                [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
                [System.Diagnostics.DebuggerDisplay("\\{ms-resource:///Resource/String2/aa\\}")]
                private sealed class aa__hzfn5FNO : global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.String2.Iaa
                {
                    global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider.this[string resourceKey]
                    {
                        get
                        {
                            if(resourceKey == null)
                                throw new global::System.ArgumentNullException();
                            return new global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider("ms-resource:///Resource/String2/aa/" + resourceKey);
                        }
                    }

                    string global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider.GetValue(string resourceKey)
                    {
                        if(resourceKey == null)
                            return global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/String2/aa");
                        return global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/String2/aa/" + resourceKey);
                    }

                    string global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.String2.Iaa.bb
                        => global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/String2/aa/bb");
                }

            }

            global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.Iha_h_a2 global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResource.ha_h_a2 { get; } = new global::ResourceGenerator.LocalizedStrings.Resource__6qftPz0F.ha_h_a2__LmVAn1fC();

            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
            [System.Diagnostics.DebuggerDisplay("\\{ms-resource:///Resource/ha.h\"a2\\}")]
            private sealed class ha_h_a2__LmVAn1fC : global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.Iha_h_a2
            {
                global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider.this[string resourceKey]
                {
                    get
                    {
                        if(resourceKey == null)
                            throw new global::System.ArgumentNullException();
                        return new global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider("ms-resource:///Resource/ha.h\"a2/" + resourceKey);
                    }
                }

                string global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider.GetValue(string resourceKey)
                {
                    if(resourceKey == null)
                        return global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/ha.h\"a2");
                    return global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/ha.h\"a2/" + resourceKey);
                }

                global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.ha_h_a2.Ib global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.Iha_h_a2.b { get; } = new global::ResourceGenerator.LocalizedStrings.Resource__6qftPz0F.ha_h_a2__LmVAn1fC.b__Fdg_GNew();

                [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
                [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
                [System.Diagnostics.DebuggerDisplay("\\{ms-resource:///Resource/ha.h\"a2/b\\}")]
                private sealed class b__Fdg_GNew : global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.ha_h_a2.Ib
                {
                    global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider.this[string resourceKey]
                    {
                        get
                        {
                            if(resourceKey == null)
                                throw new global::System.ArgumentNullException();
                            return new global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider("ms-resource:///Resource/ha.h\"a2/b/" + resourceKey);
                        }
                    }

                    string global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider.GetValue(string resourceKey)
                    {
                        if(resourceKey == null)
                            return global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/ha.h\"a2/b");
                        return global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/ha.h\"a2/b/" + resourceKey);
                    }

                    global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.ha_h_a2.b.Ihaha3 global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.ha_h_a2.Ib.haha3 { get; } = new global::ResourceGenerator.LocalizedStrings.Resource__6qftPz0F.ha_h_a2__LmVAn1fC.b__Fdg_GNew.haha3__NytdOC6r();

                    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
                    [System.Diagnostics.DebuggerDisplay("\\{ms-resource:///Resource/ha.h\"a2/b/haha3\\}")]
                    private sealed class haha3__NytdOC6r : global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.ha_h_a2.b.Ihaha3
                    {
                        global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider.this[string resourceKey]
                        {
                            get
                            {
                                if(resourceKey == null)
                                    throw new global::System.ArgumentNullException();
                                return new global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider("ms-resource:///Resource/ha.h\"a2/b/haha3/" + resourceKey);
                            }
                        }

                        string global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider.GetValue(string resourceKey)
                        {
                            if(resourceKey == null)
                                return global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/ha.h\"a2/b/haha3");
                            return global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/ha.h\"a2/b/haha3/" + resourceKey);
                        }

                        string global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.ha_h_a2.b.Ihaha3.cc
                            => global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/ha.h\"a2/b/haha3/cc");
                    }

                    string global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.ha_h_a2.Ib.c
                        => global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/ha.h\"a2/b/c");
                }

                string global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.Iha_h_a2.a
                    => global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/ha.h\"a2/a");
            }

            global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.Itest global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResource.test { get; } = new global::ResourceGenerator.LocalizedStrings.Resource__6qftPz0F.test__sNFC9KZF();

            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
            [System.Diagnostics.DebuggerDisplay("\\{ms-resource:///Resource/test\\}")]
            private sealed class test__sNFC9KZF : global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.Itest
            {
                global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider.this[string resourceKey]
                {
                    get
                    {
                        if(resourceKey == null)
                            throw new global::System.ArgumentNullException();
                        return new global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider("ms-resource:///Resource/test/" + resourceKey);
                    }
                }

                string global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider.GetValue(string resourceKey)
                {
                    if(resourceKey == null)
                        return global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/test");
                    return global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/test/" + resourceKey);
                }

                global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.test.I__Empty__ global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.Itest.__Empty__ { get; } = new global::ResourceGenerator.LocalizedStrings.Resource__6qftPz0F.test__sNFC9KZF.__kiw_1eg9();

                [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
                [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
                [System.Diagnostics.DebuggerDisplay("\\{ms-resource:///Resource/test/\\}")]
                private sealed class __kiw_1eg9 : global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.test.I__Empty__
                {
                    global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider.this[string resourceKey]
                    {
                        get
                        {
                            if(resourceKey == null)
                                throw new global::System.ArgumentNullException();
                            return new global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider("ms-resource:///Resource/test//" + resourceKey);
                        }
                    }

                    string global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider.GetValue(string resourceKey)
                    {
                        if(resourceKey == null)
                            return global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/test/");
                        return global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/test//" + resourceKey);
                    }

                    string global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.test.I__Empty__.test
                        => global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/test//test");
                }

            }

            global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.IGetValue global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResource.GetValue { get; } = new global::ResourceGenerator.LocalizedStrings.Resource__6qftPz0F.GetValue__q7u2423e();

            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
            [System.Diagnostics.DebuggerDisplay("\\{ms-resource:///Resource/GetValue\\}")]
            private sealed class GetValue__q7u2423e : global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.IGetValue
            {
                global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider.this[string resourceKey]
                {
                    get
                    {
                        if(resourceKey == null)
                            throw new global::System.ArgumentNullException();
                        return new global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider("ms-resource:///Resource/GetValue/" + resourceKey);
                    }
                }

                string global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider.GetValue(string resourceKey)
                {
                    if(resourceKey == null)
                        return global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/GetValue");
                    return global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/GetValue/" + resourceKey);
                }

                string global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.IGetValue.GetValue
                    => global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/GetValue/GetValue");
            }

            global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.Idouble global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResource.@double { get; } = new global::ResourceGenerator.LocalizedStrings.Resource__6qftPz0F.double__I0EihzXf();

            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Resource Generator", "2.0.2.0")]
            [System.Diagnostics.DebuggerDisplay("\\{ms-resource:///Resource/double\\}")]
            private sealed class double__I0EihzXf : global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.Idouble
            {
                global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider.this[string resourceKey]
                {
                    get
                    {
                        if(resourceKey == null)
                            throw new global::System.ArgumentNullException();
                        return new global::ResourceGenerator.ResourceGenerator_ResourceInfo.GeneratedResourceProvider("ms-resource:///Resource/double/" + resourceKey);
                    }
                }

                string global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResourceProvider.GetValue(string resourceKey)
                {
                    if(resourceKey == null)
                        return global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/double");
                    return global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/double/" + resourceKey);
                }

                string global::ResourceGenerator.ResourceGenerator_ResourceInfo.Resource.Idouble.@string
                    => global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/double/string");
            }

            string global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResource.Equals
                => global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/Equals");
            string global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResource.@int
                => global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/int");
            string global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResource.String1
                => global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/String1");
            string global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResource.__11_
                => global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/<11>");
            string global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResource.ha_ha
                => global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/ha\"ha");
            string global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResource.__
                => global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/\t");
            string global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResource.Reset
                => global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/Reset");
            string global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResource.MemberwiseClone
                => global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/MemberwiseClone");
            string global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResource.ReferenceEquals
                => global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/ReferenceEquals");
            string global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResource.GetType
                => global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/GetType");
            string global::ResourceGenerator.ResourceGenerator_ResourceInfo.IResource.GetHashCode
                => global::ResourceGenerator.LocalizedStrings.GetValue("ms-resource:///Resource/GetHashCode");
        }
    }
}
