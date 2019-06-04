# Experimento Huertos

![Pantalla principal](<https://raw.githubusercontent.com/CarlosManuelRodr/ExperimentoHuertos/master/img/1.png>)

![main screen](<https://raw.githubusercontent.com/CarlosManuelRodr/ExperimentoHuertos/master/img/3.png>)

## Requisitos

Se recomienda utilizar una computadora con Windows 10. El juego debe ser ejecutado con al menos dos mouse conectados. En caso de que el programa no detecte el movimiento de ningún cursor, instalar el paquete [Microsoft Visual C++ Redistributable 2019](https://aka.ms/vs/16/release/VC_redist.x64.exe>).

## Configuración de los niveles

El usuario tiene acceso a la pantalla de configuración de nivel.

![configuracion](<https://raw.githubusercontent.com/CarlosManuelRodr/ExperimentoHuertos/master/img/2.png>)

Los niveles disponibles y sus parámetros se especifican en el archivo `Levels.xml` que se encuentra en el mismo directorio que el programa ejecutable.

![xml](<https://raw.githubusercontent.com/CarlosManuelRodr/ExperimentoHuertos/master/img/4.png>)

Este archivo debe ser editado por los experimentadores a fin de ajustar los parámetros de cada nivel. En caso de que el experimentador no desee permitir al usuario que modifique los parámetros del nivel, se debe cambiar el valor de la variable `lockLevel` a `false`.

