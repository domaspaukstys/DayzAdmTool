   M S B A M L   `   `   `  ����L  HDayzServerManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:DayzServerManagerDayzServerManager  5      "    DayzServerManager.MapControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      =dayzServerManagerclr-namespace:DayzServerManager     5      .��5	      �   � RowDefinitions  5
   
   �� 5      $
��20px �6   6,   �� 5      $��* �6   6)    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6   .+�5   
   �� -   5         ��Name$ _sliderScale��5       �� TickFrequency$	 0.1Q�6'    � Minimum$	 0.1Q�6;    � Maximum$ 2Q�6I   $��2��6U    � Value 5      �� 5       �� ElementName$ _scrollViewer��6    �� Path$
 Zoom�66   	 �� Mode$	 TwoWay=�6B   5      0   *DayzServerManager.ScrollViewerMouseHandled  5   
   -   5        
  Name$
 _scrollViewer��5   5   $��1��6L   $��Hidden=�6Y   $��Hidden=�6�   $��2��6�   .��5      �� -   5        $��
_canvasMap��5      ��6'   �� $ 	_imageMap��$ ActualWidth���6`   �� $ 	_imageMap��$ ActualHeight�.+�5      �� -   5         ��Name$ 	_imageMap��5      $
��None=�6+   $��map.jpg��6:   5      5      5      5      