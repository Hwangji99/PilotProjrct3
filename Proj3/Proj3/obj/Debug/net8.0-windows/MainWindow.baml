   M S B A M L   `   `   `  ����@  <Proj3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:Proj3.ViewModelProj3.ViewModel  5   	       Proj3.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:Proj3.ViewModel     5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)    � DataContext 5	      #   Proj3.ViewModel.MainViewModel  5
   
   .��5      �  � ColumnDefinitions 5   
   �� 5      $
��0.8* �6   �� 5      $
��0.2* �6   .+�5   
   � $��0��6   .+�5      �� $��Uniform=�64   $��24,28,24,28q�6F   ��6   �� *DisplayImage+6F   �� 5      -   5         ��Name$ 	inkCanvas��5       �� 
Background
 ���� $��Stretch=�6D   $��Stretch=�6`    �� EditingMode 5      �� *InkCanvasEditingMode+5      �� 5   
   $��1��6   $��Center=�6%   $��Center=�6@   .+�5      �� $��파일 검색��6   $	��100��6%   ��61   �� *OpenFileBtn+�� 5      $��	그리기��6   $	��100��6#   ��6/   �� *
DrawingBtn+�� 5      $��	지우기��6   $	��100��6#   ��6/   �� *	ErasorBtn+�� 5      $��저장��6   $	��100��6"   ��6.   �� *	SaveBtn+5      5      5       