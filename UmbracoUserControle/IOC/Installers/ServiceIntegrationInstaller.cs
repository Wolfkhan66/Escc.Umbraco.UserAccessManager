﻿//using Castle.MicroKernel.Registration;
//using Castle.MicroKernel.SubSystems.Configuration;
//using Castle.Windsor;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using UmbracoUserControl.Services;

//namespace UmbracoUserControl.Plumbing.Installers
//{
//    public class ServiceIntegrationInstaller : IWindsorInstaller
//    {
//        public void Install(IWindsorContainer container, IConfigurationStore store)
//        {
//            container.Register(Component.For<ServicesIntegration>()
//                                        .ImplementedBy<ServicesIntegration>()
//                                        .LifestyleTransient());
//        }
//    }
//}