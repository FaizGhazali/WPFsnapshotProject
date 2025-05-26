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
    public interface ITabUCVMFactory
    {
        TabUCVM Create(Project project);
        TabUC Create();
    }

}
