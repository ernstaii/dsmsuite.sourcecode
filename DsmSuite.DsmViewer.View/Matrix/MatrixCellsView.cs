﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DsmSuite.DsmViewer.ViewModel.Matrix;

namespace DsmSuite.DsmViewer.View.Matrix
{
    public class MatrixCellsView : MatrixFrameworkElement
    {
        private MatrixViewModel _viewModel;
        private readonly RenderTheme _renderTheme;
        private Rect _rect;
        private int? _hoveredRow;
        private int? _hoveredColumn;
        private double _pitch;
        private double _offset;

        public MatrixCellsView()
        {
            _renderTheme = new RenderTheme(this);
            _rect = new Rect(new Size(_renderTheme.MatrixCellSize, _renderTheme.MatrixCellSize));
            _hoveredRow = null;
            _hoveredColumn = null;
            _pitch = _renderTheme.MatrixCellSize + 2.0;
            _offset = 1.0;

            DataContextChanged += OnDataContextChanged;
            MouseMove += OnMouseMove;
            MouseDown += OnMouseDown;
            MouseLeave += OnMouseLeave;
        }

        private void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            _viewModel = DataContext as MatrixViewModel;
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged += OnPropertyChanged;
                InvalidateVisual();
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            int row = GetHoveredRow(e.GetPosition(this));
            int column = GetHoveredColumn(e.GetPosition(this));
            if ((_hoveredRow != row) || (_hoveredColumn != column))
            {
                _hoveredRow = row;
                _hoveredColumn = column;
                _viewModel.HoverCell(row, column);
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _viewModel.HoverCell(null, null);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            int row = GetHoveredRow(e.GetPosition(this));
            int column = GetHoveredColumn(e.GetPosition(this));
            _viewModel.SelectCell(row, column);
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == nameof(MatrixViewModel.MatrixSize)) ||
                (e.PropertyName == nameof(MatrixViewModel.HoveredRow)) ||
                (e.PropertyName == nameof(MatrixViewModel.SelectedRow)) ||
                (e.PropertyName == nameof(MatrixViewModel.HoveredColumn)) ||
                (e.PropertyName == nameof(MatrixViewModel.SelectedColumn)))
            {
                InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (_viewModel != null)
            {
                int matrixSize = _viewModel.MatrixSize;
                for (int row = 0; row < matrixSize; row++)
                {
                    for (int column = 0; column < matrixSize; column++)
                    {
                        _rect.X = _offset + column * _pitch;
                        _rect.Y = _offset + row * _pitch;

                        bool isHovered = (_viewModel.HoveredRow.HasValue && (row == _viewModel.HoveredRow.Value)) ||
                                         (_viewModel.HoveredColumn.HasValue && (column == _viewModel.HoveredColumn.Value));
                        bool isSelected = (_viewModel.SelectedRow.HasValue && (row == _viewModel.SelectedRow.Value)) ||
                                          (_viewModel.SelectedColumn.HasValue && (column == _viewModel.SelectedColumn.Value));
                        MatrixColor color = _viewModel.CellColors[row][column];
                        SolidColorBrush background = _renderTheme.GetBackground(color, isHovered, isSelected);

                        dc.DrawRectangle(background, null, _rect);

                        int weight = _viewModel.CellWeights[row][column];
                        if (weight > 0)
                        {
                            Point location = new Point
                            {
                                X = 1.0 + column * _pitch,
                                Y = 14.0 + row * _pitch
                            };
                            DrawText(dc, location, weight.ToString(), _rect.Width - 2.0);
                        }
                    }
                }
                Height = _renderTheme.MatrixCellSize * matrixSize + 2.0;
                Width = Height;
            }
        }

        private int GetHoveredRow(Point location)
        {
            double row = (location.Y - _offset) / _pitch;
            return (int)row;
        }

        private int GetHoveredColumn(Point location)
        {
            double column = (location.X - _offset) / _pitch;
            return (int)column;
        }
    }
}
