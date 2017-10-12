//using system;
//using system.collections.generic;
//using system.componentmodel.composition;
//using system.componentmodel.composition.hosting;
//using system.io;
//using system.linq;
//using system.reflection;

//namespace lendsum.crosscutting.common.ioc
//{
//    /// <summary>
//    /// class that uses mef to find all registration containers.
//    /// </summary>
//    public class registrationcontainerinspector
//    {
//        /// <summary>
//        /// initializes a new instance of the <see cref="registrationcontainerinspector"/> class.
//        /// </summary>
//        public registrationcontainerinspector(string folderpath = "")
//        {
//            try
//            {
//                if (this.registrationcontainers == null || !this.registrationcontainers.any())
//                {
//                    //step 1: initializes a new instance of the
//                    //        system.componentmodel.composition.hosting.assemblycatalog
//                    //        class with the current executing assembly.
//                    using (var catalog = new aggregatecatalog())
//                    {
//                        if (string.isnullorwhitespace(folderpath))
//                        {
//                            folderpath = path.getdirectoryname(assembly.getexecutingassembly().location);
//                        }

//                        catalog.catalogs.add(new directorycatalog(folderpath));

//                        //step 2: the assemblies obtained in step 1 are added to the
//                        //compositioncontainer
//                        using (var container = new compositioncontainer(catalog))
//                        {
//                            //step 3: composable parts are created here i.e.
//                            //the import and export components
//                            //        assemble here
//                            container.composeparts(this);
//                        }
//                    }
//                }
//            }
//            catch (exception ex)
//            {
//                lendsumexception exception = new lendsumexception("there is a problem inspecting the assembly to discover the registration containers", ex);
//                throw exception;
//            }
//        }

//        /// <summary>
//        /// gets or sets registration containers.
//        /// </summary>
//        [importmany(typeof(iregistrationcontainer))]
//        public ienumerable<iregistrationcontainer> registrationcontainers { get; set; }
//    }
//}