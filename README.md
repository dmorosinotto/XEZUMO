XEZUMO
======

Materiale del Corso **XE.Net Azure Mobile Services Deep Dive** del 05/12/2014 by Daniele Morosinotto

Corso "avanzato" su l'utilizzo degli [Azure Mobile Services](http://azure.microsoft.com/it-it/services/mobile-services/)

#### SLIDES
E' possibile visualizzare le slide direttamente [online](http://1drv.ms/1tyN3bC)

#### CODICE
Per poter utilizzare il codice è necessario: 
- Avere una sottoscrizione Azure (va benissimo anche la [Trial](http://azure.microsoft.com/it-it/pricing/free-trial/))
- Accedere al [Portale Azure](https://manage.windowsazure.com/) e creare un nuovo **Mobile Service**
- Sostituire [nei](zumoiot_Windows_CS/zumoiot/zumoiot.Shared/App.xaml.cs) [files](zumoiot_Html/page.js) i riferimenti all'**Url** e **ApplicationKey** del vostro servizio, ricavandoli dalla Dashboard

#### NOTE
Questa repo contiene 3 cartelle:
- [zumoiot](zumoiot/) codice del backend nodejs (deve essere pubblicato tramite il controllo codice sorgente: repo GIT del vs MobileService [vedi istruzioni qui](http://msdn.microsoft.com/library/azure/dn280976.aspx))
- [zumoiot_Html](zumoiot_Html/) esempio di codice client in HTML5/JS (per visualizzarlo lanciare lo [script](zumoiot_Html/server/) e navigare [http://localhost:8000](http://localhost:8000)
- [zumoiot_Windows_CS](zumoiot_Windows_CS/) esempio di codice client in C# (Universal App che gira sia su Windows 8 che su Windows Phone)

Inoltre sono presenti dei branch, che corrispondono all'evoluzione del codice dalla versione iniziale del progetto (**step0** come scaricata dalla guida Get Started) fino ad arrivare all'implementazione completa (**step5** del client e server)   
