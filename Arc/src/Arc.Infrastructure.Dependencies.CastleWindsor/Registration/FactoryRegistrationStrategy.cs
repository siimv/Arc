#region License
//
//   Copyright 2009 Marek Tihkan
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License
//
#endregion

using Arc.Infrastructure.Dependencies.CastleWindsor.Extensions;
using Castle.MicroKernel.Registration;
using IRegistration=Arc.Infrastructure.Dependencies.Registration.IRegistration;

namespace Arc.Infrastructure.Dependencies.CastleWindsor.Registration
{
    internal class FactoryRegistrationStrategy : BaseRegistrationStrategy
    {
        public FactoryRegistrationStrategy(IRegistration registration, ServiceLocator serviceLocator) 
            : base(registration, serviceLocator)
        {
        }

        public override void Register()
        {
			ServiceLocator.Container.Register(
				Component.For(Registration.ServiceType)
					.UsingFactoryMethod(() => Registration.Factory.Invoke(ServiceLocator))
					.LifeStyle.Is(LifeStyleFactory.Create(Registration.Scope)));
        }
    }
}