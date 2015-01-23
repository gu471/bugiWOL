Die ausführbare Datei befindet sich im Ordner /exe/.

Im Ordner MAC befinden sich Beispiele zur Verwendung.
Der Ordner "MAC" muss sich exe-Ordner befinden. In im sind Dateien im folgenden Format anzulegen:
<blockquote><b>ferien.<i>MAC-Adresse<i></b></blockquote>
Die MAC-Adresse darf keine Trennzeichen enthalten.

Diese Dateien sind wie folgt zu füllen:

<blockquote>
Erster Schultag<br>
<i>Start Ferien</i>, <i>Ende Ferien</i><br>
...<br>
<i>Start Ferien</i>, <i>Ende Ferien</i><br>
Letzter Schultag<br>
</blockquote>

Mit dem Aufruf "startWOL.exe -create" werden alle Dateien "ferien.<i>MAC</i>" überprüft und eine Datei "wol.<i>MAC</i>" angelegt, in der alle Daten liegen, an denen an die MAC-Adresse ein WOL-Packet gesendet werden soll.
Zum Erzeugen kann auch die beiliegende .bat verwendet werden.
