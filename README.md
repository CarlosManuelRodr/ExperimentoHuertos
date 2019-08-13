# Experimento Huertos

![Pantalla principal](<https://raw.githubusercontent.com/CarlosManuelRodr/ExperimentoHuertos/master/img/1.png>)

![main screen](<https://raw.githubusercontent.com/CarlosManuelRodr/ExperimentoHuertos/master/img/3.png>)

## Descargar
* [Ejecutable de Windows](https://github.com/CarlosManuelRodr/ExperimentoHuertos/releases/download/v1.1/ExperimentoHuertos.zip)

## Requisitos

Se recomienda utilizar una computadora con Windows 10. El juego debe ser ejecutado con al menos dos mouse conectados. En caso de que el programa no detecte el movimiento de ningún cursor, instalar el paquete [Microsoft Visual C++ Redistributable 2019](https://aka.ms/vs/16/release/VC_redist.x64.exe>).

Para usar el analizador de datos es necesario instalar [Mathematica](http://www.wolfram.com/mathematica/).

## Estructura del proyecto

En el directorio

<Directorio del juego>/Levels

se encuentra el archivo de configuración de los niveles que puede ser editado por los experimentadores.

Los datos de salida son guardados por defecto en

<Directorio de usuario>/HuertosLog

y se puede cambiar en cualquier momento desde el menú de opciones dentro del juego.

También se provee una libreta de análisis de datos programada en el lenguaje Wolfram que puede ser ejecutada en Mathematica, y se encuentra en el directorio

<Directorio del juego>/DataAnalytics

## Configuración de los niveles

El usuario tiene acceso a la pantalla de configuración de nivel.

![configuracion](<https://raw.githubusercontent.com/CarlosManuelRodr/ExperimentoHuertos/master/img/2.png>)

Los niveles disponibles y sus parámetros se especifican en el archivo `Levels.xml` que se encuentra en el mismo directorio que el programa ejecutable.

![xml](<https://raw.githubusercontent.com/CarlosManuelRodr/ExperimentoHuertos/master/img/4.png>)

Este archivo debe ser editado por los experimentadores a fin de ajustar los parámetros de cada nivel. Asimismo es posible seleccionar una imagen que actúe como identificador del nivel, la cual puede ser modificada en la etiqueta <image>.

