import { MaterialCommunityIcons, MaterialIcons } from "@expo/vector-icons";
import { usePathname, useRouter } from "expo-router";
import { MotiView } from "moti";
import React from "react";
import { Pressable, Text, View } from "react-native";
import i18n from "../utils/translations";

const Footer = () => {
  const router = useRouter();
  const pathname = usePathname();

  const ACTIVE_COLOR = "#1544b2";
  const INACTIVE_COLOR = "#94a3b8";

  const navItems = [
    {
      id: "home",
      label: i18n.t("nav_home"),
      icon: "home",
      family: "MaterialIcons",
      path: "/dashboard",
    },
    {
      id: "stats",
      label: i18n.t("nav_stats"),
      icon: "leaderboard",
      family: "MaterialIcons",
      path: "/stats",
    },
    {
      id: "learn",
      label: i18n.t("nav_learn"),
      icon: "controller",
      family: "MaterialCommunityIcons",
      path: "/learn",
    },
    {
      id: "school",
      label: i18n.t("nav_school"),
      icon: "school",
      family: "MaterialIcons",
      path: "/school",
    },
    {
      id: "profile",
      label: i18n.t("nav_profile"),
      icon: "account-circle",
      family: "MaterialIcons",
      path: "/exam/examHistory",
    },
  ];

  return (
    <View className="absolute bottom-0 left-0 right-0 bg-white/95 dark:bg-slate-900/95 border-t border-slate-100 dark:border-slate-800 px-4 pb-8 pt-2 flex-row justify-around items-center shadow-lg">
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
            className="items-center justify-center py-2 flex-1"
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
                size={26}
                color={active ? ACTIVE_COLOR : INACTIVE_COLOR}
              />
            </MotiView>

            <Text
              style={{
                fontSize: 10,
                fontWeight: "700",
                marginTop: 4,
                color: active ? ACTIVE_COLOR : INACTIVE_COLOR,
                opacity: active ? 1 : 0.7,
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
