import { MaterialCommunityIcons, MaterialIcons } from "@expo/vector-icons";
import { usePathname, useRouter } from "expo-router";
import { MotiView } from "moti";
import React from "react";
import { Platform, Pressable, Text, View } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import i18n from "../utils/translations";

const Footer = () => {
  const router = useRouter();
  const pathname = usePathname();
  const insets = useSafeAreaInsets();

  const ACTIVE_COLOR = "#1544b2";
  const INACTIVE_COLOR = "#94a3b8";

  const footerPlatformStyles = Platform.select({
    ios: {
      position: "absolute" as const,
      bottom: 0,
      left: 0,
      right: 0,
      paddingBottom: insets.bottom > 0 ? insets.bottom : 20,
    },
    android: {
      position: "relative" as const,
      paddingBottom: 16,
    },
    default: {
      position: "relative" as const,
      paddingBottom: 20,
    },
  });

  const navItems = [
    {
      id: "home",
      label: i18n.t("navigation.nav_home"),
      icon: "home",
      family: "MaterialIcons",
      path: "/dashboard",
    },
    {
      id: "stats",
      label: i18n.t("navigation.nav_stats"),
      icon: "leaderboard",
      family: "MaterialIcons",
      path: "/user/stats",
    },
    {
      id: "learn",
      label: i18n.t("navigation.nav_learn"),
      icon: "controller",
      family: "MaterialCommunityIcons",
      path: "/learning/studyTopics",
    },
    {
      id: "profile",
      label: i18n.t("navigation.nav_profile"),
      icon: "account-circle",
      family: "MaterialIcons",
      path: "/user/profile",
    },
  ];

  return (
    <View
      style={[footerPlatformStyles, { paddingTop: 12 }]}
      className="bg-white/95 dark:bg-slate-900/95 border-t border-slate-100 dark:border-slate-800 px-4 flex-row justify-around items-center shadow-[0_-4px_10px_rgba(0,0,0,0.05)]"
    >
      {navItems.map((item) => {
        const active = pathname === item.path;
        const IconComponent =
          item.family === "MaterialCommunityIcons"
            ? MaterialCommunityIcons
            : MaterialIcons;

        return (
          <Pressable
            key={item.id}
            onPress={() => {
              if (!active) {
                router.replace(item.path as any);
              }
            }}
            className="items-center justify-center flex-1 py-1"
          >
            <MotiView
              animate={{
                scale: active ? 1.1 : 1,
                translateY: active ? -2 : 0,
              }}
              transition={{
                type: "timing",
                duration: 150,
              }}
            >
              <IconComponent
                name={item.icon as any}
                size={24}
                color={active ? ACTIVE_COLOR : INACTIVE_COLOR}
              />
            </MotiView>

            <Text
              style={{
                fontSize: 10,
                fontWeight: "700",
                marginTop: 4,
                color: active ? ACTIVE_COLOR : INACTIVE_COLOR,
              }}
            >
              {item.label}
            </Text>

            {active && (
              <MotiView
                from={{ opacity: 0, scale: 0 }}
                animate={{ opacity: 1, scale: 1 }}
                className="absolute -bottom-1 w-1 h-1 rounded-full bg-[#1544b2]"
              />
            )}
          </Pressable>
        );
      })}
    </View>
  );
};

export default Footer;
