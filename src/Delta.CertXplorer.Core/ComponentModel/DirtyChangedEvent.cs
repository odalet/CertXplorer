using System;
using System.ComponentModel;

namespace Delta.CertXplorer.ComponentModel
{
    /// <summary>
    /// Repr�sente la m�thode qui traite les �v�nements <see cref="IDirtyNotifier.DirtyChanged"/>.
    /// </summary>
    /// <param name="sender">Source de l'�v�nement.</param>
    /// <param name="e">Donn�es de l'�v�nement.</param>
    public delegate void DirtyChangedEventHandler(object sender, DirtyChangedEventArgs e);

    /// <summary>
    /// Fournit des donn�es pour l'�v�nement <see cref="IDirtyNotifier.DirtyChanged"/>.
    /// </summary>
    public class DirtyChangedEventArgs : PropertyChangedEventArgs
    {
        private bool dirty = false;

        /// <summary>
        /// Construit un objet <see cref="DirtyChangedEventArgs"/>
        /// </summary>
        /// <param name="isDirty">Valeur de la propri�t� <see cref="Dirty"/>.</param>
        public DirtyChangedEventArgs(bool isDirty) : base("Dirty") { dirty = isDirty; }

        /// <summary>
        /// Obtient une valeur indiquant si l'�tat de l'objet source de 
        /// l'�v�nement a chang� et n�cessite une sauvegarde.
        /// </summary>
        public bool Dirty { get { return dirty; } }
    }
}
