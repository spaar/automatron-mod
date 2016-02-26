package spaar.mods.keysimulator;

import java.awt.*;
import java.awt.event.KeyEvent;
import java.io.InputStreamReader;
import java.io.Reader;
import java.nio.charset.StandardCharsets;
import java.util.Scanner;

public class KeySimulator {

  public static void main(String[] args) {
    try {
      Robot robot = new Robot();
      Scanner in = new Scanner(new InputStreamReader(System.in, StandardCharsets.US_ASCII));

      while (true) {
        while (!in.hasNextLine())
          ;

        String input = in.nextLine();

        String keys = input.split(":")[1];
        String mode = input.split(":")[0];

        for (char c : keys.toCharArray()) {
          int keyCode = KeyEvent.getExtendedKeyCodeForChar(c);

          if (mode.equals("p")) {
            robot.keyPress(keyCode);
            robot.keyRelease(keyCode);
          } else if (mode.equals("h")) {
            robot.keyPress(keyCode);
          } else if (mode.equals("r")) {
            robot.keyRelease(keyCode);
          }
        }
      }
    } catch (Exception e) {
      e.printStackTrace();
      while (true) ;
    }
  }

}
