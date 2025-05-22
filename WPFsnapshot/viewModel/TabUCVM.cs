using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFsnapshot.model;
using WPFsnapshot.services;

namespace WPFsnapshot.viewModel
{
    public class TabUCVM
    {
        public Project Project { get; set; }
        public Project Clone { get; set; }

        public Game Game { get; set; } = new Game
        {
            Guid = Guid.NewGuid(),
            Name = "Narnia",
            Description = "Adventure through a magical world."
        };
        public Game GameClone { get; set; }
        public UndoRedoManager UndoRedo { get; } = new();

        public void HandleClone()
        {
            Clone = Project.Clone();
        }
        public void HandleCloneGame()
        {
            GameClone = Game.Clone();
        }
        public void RenameProject()
        {
            
            var oldName = Clone.Name;
            var newName = Project.Name;
            var action = new PropertyChangeAction<string>(
                () => Project.Name,
                val => Project.Name = val,
                oldName,
                newName);

            UndoRedo.Execute(action);
        }
        public void RenameGame()
        {

            var oldName = GameClone.Name;
            var newName = Game.Name;
            var action = new PropertyChangeAction<string>(
                () => Game.Name,
                val => Game.Name = val,
                oldName,
                newName);

            UndoRedo.Execute(action);
        }
    }
}
