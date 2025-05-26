using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFsnapshot.model;
using WPFsnapshot.view;
using WPFsnapshot.viewModel;

namespace WPFsnapshot.factories
{
    public class TabUCVMFactory : ITabUCVMFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public TabUCVMFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TabUCVM Create(Project project)
        {
            var vm = _serviceProvider.GetRequiredService<TabUCVM>();
            vm.Project = project;
            return vm;
        }
        public TabUC Create()
        {
            return _serviceProvider.GetRequiredService<TabUC>();
        }
    }
}
