﻿using System.Windows;
using DsmSuite.DsmViewer.ViewModel.Matrix;
using System.Windows.Input;
using System.Windows.Media;

namespace DsmSuite.DsmViewer.View.Matrix
{
    public class MatrixRowMetricsView : MatrixFrameworkElement
    {
        private MatrixViewModel _viewModel;
        private readonly MatrixTheme _theme;
        private Rect _rect;
        private int? _hoveredRow;
        private readonly double _pitch;
        private readonly double _offset;
        
        public MatrixRowMetricsView()
        {
            _theme = new MatrixTheme(this);
            _rect = new Rect(new Size(_theme.MatrixMetricsViewWidth, _theme.MatrixCellSize));
            _hoveredRow = null;
            _pitch = _theme.MatrixCellSize + _theme.SpacingWidth;
            _offset = _theme.SpacingWidth / 2;

            DataContextChanged += OnDataContextChanged;
            MouseMove += OnMouseMove;
            MouseDown += OnMouseDown;
            MouseLeave += OnMouseLeave;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
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
            if (_hoveredRow != row)
            {
                _hoveredRow = row;
                _viewModel.HoverRow(row);
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _viewModel.HoverColumn(null);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            int row = GetHoveredRow(e.GetPosition(this));
            _viewModel.SelectRow(row);
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MatrixViewModel.ColumnHeaderToolTipViewModel))
            {
                ToolTip = _viewModel.ColumnHeaderToolTipViewModel;
            }

            if ((e.PropertyName == nameof(MatrixViewModel.MatrixSize)) ||
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
                    _rect.X = 0;
                    _rect.Y = _offset + row * _pitch;

                    bool isHovered = row == _viewModel.HoveredRow?.Index;
                    bool isSelected = row == _viewModel.SelectedRow?.Index;
                    MatrixColor color = _viewModel.ColumnColors[row];
                    SolidColorBrush background = _theme.GetBackground(color, isHovered, isSelected);

                    dc.DrawRectangle(background, null, _rect);

                    string content = _viewModel.Metrics[row];
                    double textWidth = MeasureText(content);
                    Point texLocation = new Point(_rect.X + _rect.Width - 30.0 - textWidth, _rect.Y + 15.0);
                    DrawText(dc, content, texLocation, _theme.TextColor, _rect.Width - _theme.SpacingWidth);
                }

                Height = _theme.MatrixHeaderHeight + _theme.SpacingWidth;
                Width = _theme.MatrixCellSize * matrixSize + _theme.SpacingWidth;
            }
        }

        private int GetHoveredRow(Point location)
        {
            double row = (location.Y - _offset) / _pitch;
            return (int)row;
        }
    }

}
