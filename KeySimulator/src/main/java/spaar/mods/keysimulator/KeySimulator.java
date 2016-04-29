package spaar.mods.keysimulator;

import java.awt.*;
import java.awt.event.KeyEvent;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.io.PrintStream;
import java.nio.charset.StandardCharsets;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Map;
import java.util.Scanner;
import java.util.concurrent.TimeUnit;

public class KeySimulator {

  public static Map<String, Integer> SpecialKeys = new HashMap<String, Integer>();

  private static int exceptionCount = 0;
  private static final int maxExceptionCount = 5;

  static {
    SpecialKeys.put("alt", KeyEvent.VK_ALT);
    SpecialKeys.put("ctrl", KeyEvent.VK_CONTROL);
    SpecialKeys.put("shift", KeyEvent.VK_SHIFT);
    SpecialKeys.put("left arrow", KeyEvent.VK_LEFT);
    SpecialKeys.put("right arrow", KeyEvent.VK_RIGHT);
    SpecialKeys.put("up arrow", KeyEvent.VK_UP);
    SpecialKeys.put("down arrow", KeyEvent.VK_DOWN);
    SpecialKeys.put("numpad 0", KeyEvent.VK_NUMPAD0);
    SpecialKeys.put("numpad 1", KeyEvent.VK_NUMPAD1);
    SpecialKeys.put("numpad 2", KeyEvent.VK_NUMPAD2);
    SpecialKeys.put("numpad 3", KeyEvent.VK_NUMPAD3);
    SpecialKeys.put("numpad 4", KeyEvent.VK_NUMPAD4);
    SpecialKeys.put("numpad 5", KeyEvent.VK_NUMPAD5);
    SpecialKeys.put("numpad 6", KeyEvent.VK_NUMPAD6);
    SpecialKeys.put("numpad 7", KeyEvent.VK_NUMPAD7);
    SpecialKeys.put("numpad 8", KeyEvent.VK_NUMPAD8);
    SpecialKeys.put("numpad 9", KeyEvent.VK_NUMPAD9);
  }

  public static void main(String[] args) throws IOException {
    try {
      long timeout = System.currentTimeMillis() + TimeUnit.SECONDS.toMillis(5);
      Robot robot = new Robot();
      Scanner in = new Scanner(new InputStreamReader(System.in, StandardCharsets.US_ASCII));
      PrintStream out = new PrintStream(System.out, false, StandardCharsets.US_ASCII.name());

      while (true) {
        while (!in.hasNextLine()) {
          if (System.currentTimeMillis() > timeout) {
            System.exit(1);
          }
        }

        String input = in.nextLine();

        if (input.equals("init")) {
          out.println("ok");
          out.flush();
          continue;
        }

        if (input.equals("ping")) {
          timeout = System.currentTimeMillis() + TimeUnit.SECONDS.toMillis(5);
          continue;
        }

        String mode = input.split(":")[0];
        String[] keys = input.split(":")[1].split(",");

        int[] keyCodes = ParseKeys(keys);

        if (mode.equals("c")) {
          boolean valid = true;
          for (int keyCode : keyCodes) {
            if (keyCode == KeyEvent.VK_UNDEFINED) {
              valid = false;
              break;
            }
          }
          if (valid) {
            out.println("v");
            out.flush();
          }
          else {
            out.println("n");
            out.flush();
          }
        }

        for (int keyCode : keyCodes) {
          if (keyCode != KeyEvent.VK_UNDEFINED) {
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
      }
    } catch (Exception e) {
      exceptionCount++;
      if (exceptionCount > maxExceptionCount) {
        System.exit(1);
      } else {
        main(args);
      }
    }
  }

  public static int[] ParseKeys(String[] keys) {
    int[] keyCodes = new int[keys.length];
    for (int i = 0; i < keys.length; i++) {
      String key = keys[i];
      int keyCode;
      if (key.length() == 1) {
        char c = key.charAt(0);
        keyCode = KeyEvent.getExtendedKeyCodeForChar(c);
      } else {
        if (SpecialKeys.containsKey(key)) {
          keyCode = SpecialKeys.get(key);
        } else {
          keyCode = KeyEvent.VK_UNDEFINED;
        }
      }
      keyCodes[i] = keyCode;
    }

    return keyCodes;
  }

}
