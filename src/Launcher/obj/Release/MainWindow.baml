   M S B A M L   `   `   `  ���� A  =Violet, Version=1.0.0.2, Culture=neutral, PublicKeyToken=null#clr-namespace:LauncherLauncher  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem     Launcher.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     (localclr-namespace:Launcher     9System&clr-namespace:System;assembly=mscorlib     -     � Title$  S4Quartz��$��656.832��$
��1064�� � 
ResizeMode$ NoResize=� � WindowStartupLocation$ CenterScreen=� � WindowStyle$
 None=� � AllowsTransparency .  J� 	AllowDrop . ����  �� ImageSource$ Res/launcherlowres.png�� j� Stretch$
 Fill=�.��� $��170,0,189,42q�$	��415��$��Bottom=� � RowDefinitions �� $	��50* ��� $
��301* �	 � ColumnDefinitions	 �� $	��71* ��� $��7* ��� $	��53* ��� $	��10* �.+�o� $��Center=�$��2��$��
114,40,0,0q�$	��Top=�$	��201��$��2��$��2�� � $�� ��$��5��
 J� Opacity$

 0.85Q�$��Bottom=�$��Bottom=�$��
70,0,1,204q�$��1��$��3�� � BindingGroup &   System.Windows.Data.BindingGroup  �� -    ��Name$ Button��$��0,0,135,235q�$
��Fill=�$��23��$��Bottom=�$��Right=�$	��110��$��1���� -    ��Name$ ButtonTx��$��110,98,160,224q�$
��Wrap=�$�� ��$��verdana)�$��15(�$��34��$��Center=�$��Center=�$��85��
�������$
��Bold"�$��1�� J� RenderTransformOrigin$ 	0.5,0.471&� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect     RenderingBias$ Quality=��� -    ��Name$ login_username��$��0��$��-2,0,138,337q� �� TextWrapping$
 Wrap=�
������ ���	#D81D1D1D
���� ��$��14(�$��Right=�$	��219��$��25��$��Bottom=�$ 0.5,0.5&�$��2��.��z�V� .~��� ��  �� AngleX$ 0.494Q��� T�  T� X$ -0.116Q�.���� -    ��Name$ 	NoticeBox��$��Stretch=�
������ $��Bottom=����	#D8000000
�������$ �� IsSynchronizedWithCurrentItem$ FalseY�$ 0.487,0.502&�$��105,0,-10,220q�$��2��$	��180��$��2��$��2��.@��� $�� ��$
��Bold"�$��Center=��� �� Welcome to S4 Violet�� � $�� ��$

 0.85Q�
������ $��Right=�$	��185��$	��705��$��Bottom=�� -    �Name$ Progessbar1��$��
70,0,1,194q� � Value$ 1Q�
���� ��$��5��$��Bottom=�
������ $��1��$��3��   � -   $ Progessbar2��$��
70,0,1,205q�
����p��$ 1Q�$��5��$��Bottom=�
������ $��1��$��3��G� -    G�Name$ login_passwd��$��1��$��-3,0,138,279q�$��Bottom=�$��28��
������ ���	#D81D1D1D$��Right=�$	��220��
���� ��$��1���� -	    ��Name$ errtx_label��$��
0,0,25,209q�$��Bottom=�
���   �$��Right=�$	��255��$��21��$��1���� -
    ��Name$
 Save��$
��Save��$��Stretch=�$��14��$��-3,0,233,260q�$��Bottom=�
������
������ $��1��$ 0.037,-0.625&��� $
��Left=�$��105,0,-5,225q�$
��Wrap=�$��Bottom=�$	��215��$��80��
������ $ 0.5,0.5&�$��2��$��1��$��2��z�V� .~��� �� $
 -0.3Q��� T� $ 0.495Q��� .F��� -   $ 
loginstats��$��Login: Online��$
��Left=�$	��Top=�$��30��$	��134��
�������$ 0.571,-0.167&��� .F��� -   $ Online��$
��1000��$
��Left=�$	��Top=�$��24��$	��134��
��������� .F��� -   $ serverstats��$��Server: Online��$
��Left=�$	��Top=�$��25��$	��134��
�������$ 	0.555,0.6&�z�V� .~��� �� $ 2.203Q��� T� $ 0.802Q��� -   $ Close��$��0,-68,-134,112q�$
��Fill=�$��Res/close.png��$��15��$��Bottom=�$��Right=�$��20��$��3���� -   $ dbgtx��$�� ��$��Bottom=�
������$��Bottom=�$��26��$��75,0,25,204q�$��1��