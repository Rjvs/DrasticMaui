﻿// <copyright file="BaseViewModel.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using DrasticMaui.Services;
using DrasticMaui.Tools;

namespace DrasticMaui.ViewModels
{
    /// <summary>
    /// Base View Model.
    /// </summary>
    public class BaseViewModel : ExtendedBindableObject, IDisposable
    {
        private bool isBusy;
        private bool isRefreshing;
        private string? title;
        private Page? originalPage;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseViewModel"/> class.
        /// </summary>
        /// <param name="services">IServiceProvider.</param>
        /// <param name="originalPage">Original Page.</param>
        public BaseViewModel(IServiceProvider services, Page? originalPage = null)
        {
            this.originalPage = originalPage;
            this.Services = services;
            var error = services.GetService<IErrorHandlerService>();
            var navigation = services.GetService<INavigationService>();

            if (navigation is null)
            {
                throw new ArgumentNullException(nameof(navigation));
            }

            if (error is null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            this.Navigation = navigation;
            this.Error = error;

            this.CloseDialogCommand = new AsyncCommand(
               async () => await this.ExecutePopModalCommand(),
               null,
               this.Error);
        }

        /// <summary>
        /// Gets or Sets a value indicating whether the view is busy.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            set
            {
                this.isBusy = value;
                this.OnPropertyChanged(nameof(this.IsBusy));
            }
        }

        /// <summary>
        /// Gets or Sets a value indicating whether the view is refreshing.
        /// </summary>
        public bool IsRefreshing
        {
            get
            {
                return this.isRefreshing;
            }

            set
            {
                this.isRefreshing = value;
                this.OnPropertyChanged(nameof(this.IsRefreshing));
            }
        }

        /// <summary>
        /// Gets or sets the page title.
        /// </summary>
        public string? Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.title = value;
                this.OnPropertyChanged(nameof(this.Title));
            }
        }

        /// <summary>
        /// Gets the Close Dialog Command.
        /// </summary>
        public AsyncCommand CloseDialogCommand { get; private set; }

        /// <summary>
        /// Gets the original page.
        /// </summary>
        protected Page? OriginalPage => this.originalPage;

        /// <summary>
        /// Gets the service provider collection.
        /// </summary>
        protected IServiceProvider Services { get; private set; }

        /// <summary>
        /// Gets the navigation service.
        /// </summary>
        protected INavigationService Navigation { get; private set; }

        /// <summary>
        /// Gets the error handler service.
        /// </summary>
        protected IErrorHandlerService Error { get; private set; }

        /// <summary>
        /// Gets the hosted window for the view model, if the original page was passed in.
        /// </summary>
        protected Window? HostedWindow => this.OriginalPage?.GetParentWindow();

        /// <summary>
        /// Gets the Visual Diagnostics Overlay for the given window this view model is contained in.
        /// </summary>
        protected IVisualDiagnosticsOverlay? VisualDiagnosticsOverlay => this.HostedWindow?.VisualDiagnosticsOverlay;

        /// <summary>
        /// Load VM Async.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        public virtual Task LoadAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Sets title for page.
        /// </summary>
        /// <param name="title">The Title.</param>
        public virtual void SetTitle(string title = "")
        {
            this.Title = title;
        }

        /// <summary>
        /// Unload VM Async.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        public virtual Task UnloadAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose VM Values.
        /// </summary>
        protected virtual void DisposeVM()
        {
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing">Is Disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.DisposeVM();
                }

                this.disposedValue = true;
            }
        }

        private async Task ExecutePopModalCommand()
        {
            if (this.HostedWindow is not null)
            {
                await this.Navigation.PopModalAsync(this.HostedWindow);
            }
        }
    }
}
