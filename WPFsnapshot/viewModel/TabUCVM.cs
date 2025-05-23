using Microsoft.Extensions.DependencyInjection;
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
        public Project Project { get; set; } = null!;
        public Project Clone { get; set; } = null!;

        public Game Game { get; set; } = new Game
        {
            Guid = Guid.NewGuid(),
            Name = "Narnia",
            Description = "Adventure through a magical world."
        };
        public Game GameClone { get; set; } = null!;
        public UndoRedoManager UndoRedo { get; } = new();

        public UndoRedoService _undoRedoService;

        public TabUCVM(UndoRedoService undoRedoService, Project p)
        {
            _undoRedoService = undoRedoService;
            
        }

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
            if (oldName != newName)
            {
                UndoRedo.Execute(action);
            }
            
            UpdateUndoRedoService();
            
        }
        public void UpdateUndoRedoService()
        {
            _undoRedoService.UndoCount = UndoRedo.UndoCount;
            _undoRedoService.RedoCount = UndoRedo.RedoCount;
        }

        public void DoUndo()
        {
            UndoRedo.Undo();
            UpdateUndoRedoService();
        }

        public void DoRedo()
        {
            UndoRedo.Redo();
            UpdateUndoRedoService();
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
