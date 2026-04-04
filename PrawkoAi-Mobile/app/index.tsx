import { FontAwesome5, MaterialIcons } from "@expo/vector-icons";
import * as AuthSession from "expo-auth-session";
import * as Device from "expo-device";
import * as WebBrowser from "expo-web-browser";
import React, { useContext, useState } from "react";
import { StatusBar, Text, TouchableOpacity, View } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { AuthContext, TokenResponse } from "./_layout";

WebBrowser.maybeCompleteAuthSession();

export default function LoginScreen() {
  const insets = useSafeAreaInsets();
  const { signIn: completeSignIn } = useContext(AuthContext);

  const [isOpen, setIsOpen] = useState(false);
  const [selectedLang, setSelectedLang] = useState({ code: "PL", flag: "🇵🇱" });

  const languages = [
    { code: "PL", flag: "🇵🇱", label: "Polski" },
    { code: "EN", flag: "🇬🇧", label: "English" },
    { code: "DE", flag: "🇩🇪", label: "Deutsch" },
    { code: "UA", flag: "🇺🇦", label: "Українська" },
  ];

  const handleGoogleSignIn = async () => {
    const redirectUri = AuthSession.makeRedirectUri({ scheme: "prawkoai" });
    const deviceId = Device.osInternalBuildId ?? "dev_id";
    const deviceName = Device.deviceName ?? "Mobile";

    const apiUrl = `${process.env.EXPO_PUBLIC_API_URL}/api/account/login/google`;
    const authUrl = `${apiUrl}?returnUrl=${encodeURIComponent(redirectUri)}&deviceId=${encodeURIComponent(deviceId)}&deviceName=${encodeURIComponent(deviceName)}`;

    try {
      const result = await WebBrowser.openAuthSessionAsync(
        authUrl,
        redirectUri,
      );
      if (result.type === "success" && result.url) {
        const url = new URL(result.url);

        const accessToken = url.searchParams.get("accessToken");
        const refreshToken = url.searchParams.get("refreshToken");

        if (accessToken && refreshToken) {
          await completeSignIn({
            accessToken,
            refreshToken,
          });
        }
      }
    } catch (error) {
      console.error("Błąd sesji przeglądarki:", error);
    }
  };

  const handleGuestSignIn = async () => {
    const deviceId = Device.osInternalBuildId ?? "dev_id";

    const apiUrl = `${process.env.EXPO_PUBLIC_API_URL}/api/account/login/guest`;

    const response = await fetch(
      `${apiUrl}?deviceId=${encodeURIComponent(deviceId)}`,
      {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
        },
      },
    );

    const data: TokenResponse = await response.json();

    if (response.ok) {
      await completeSignIn(data);
    } else {
      console.error("Błąd logowania jako gość:", data);
    }
  };

  const selectLanguage = (lang: { code: string; flag: string }) => {
    setSelectedLang(lang);
    setIsOpen(false);
  };

  return (
    <View
      className="flex-1 bg-[#f6f6f8] dark:bg-[#111621] items-center justify-center p-6"
      style={{ paddingTop: insets.top, paddingBottom: insets.bottom }}
    >
      <StatusBar barStyle="dark-content" />

      <View className="absolute -top-10 -right-10 w-64 h-64 bg-[#1544b2]/5 rounded-full blur-3xl" />
      <View className="absolute -bottom-20 -left-20 w-80 h-80 bg-[#1544b2]/5 rounded-full blur-3xl" />

      <View
        className="absolute z-50 items-end"
        style={{ top: insets.top + 10, right: 20 }}
      >
        <TouchableOpacity
          onPress={() => setIsOpen(!isOpen)}
          activeOpacity={0.8}
          className="flex-row items-center gap-2 px-3 py-2 rounded-full bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700 shadow-sm"
        >
          <Text className="text-base">{selectedLang.flag}</Text>
          <Text className="text-xs font-bold text-slate-700 dark:text-slate-200 uppercase tracking-tight">
            {selectedLang.code}
          </Text>
          <MaterialIcons
            name={isOpen ? "expand-less" : "expand-more"}
            size={18}
            color="#94a3b8"
          />
        </TouchableOpacity>

        {isOpen && (
          <View className="mt-2 w-40 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-2xl shadow-xl overflow-hidden">
            {languages.map((lang) => (
              <TouchableOpacity
                key={lang.code}
                onPress={() => selectLanguage(lang)}
                className={`flex-row items-center gap-3 px-4 py-3 border-b border-slate-50 dark:border-slate-700/50 last:border-b-0 ${
                  selectedLang.code === lang.code
                    ? "bg-slate-50 dark:bg-slate-700"
                    : ""
                }`}
              >
                <Text className="text-lg">{lang.flag}</Text>
                <View>
                  <Text className="text-sm font-bold text-slate-700 dark:text-slate-200">
                    {lang.code}
                  </Text>
                  <Text className="text-[10px] text-slate-400 uppercase font-medium">
                    {lang.label}
                  </Text>
                </View>
              </TouchableOpacity>
            ))}
          </View>
        )}
      </View>

      <View className="w-full max-w-sm items-center">
        <View className="mb-6 w-24 h-24 items-center justify-center bg-white dark:bg-slate-800 rounded-[28px] shadow-sm border border-[#1544b2]/10">
          <MaterialIcons name="auto-fix-high" size={48} color="#1544b2" />
        </View>

        <View className="items-center">
          <Text className="text-slate-900 dark:text-white text-3xl font-bold tracking-tight text-center">
            Prawko <Text className="text-[#1544b2]">AI</Text>
          </Text>
          <View className="h-1 w-6 bg-[#1544b2] rounded-full mt-1" />
        </View>
        <Text className="text-slate-500 dark:text-slate-400 text-base text-center mt-4 px-4 leading-6">
          Zacznij naukę na prawo jazdy z pomocą AI
        </Text>

        <View className="w-full gap-4 mt-4">
          <TouchableOpacity
            onPress={handleGoogleSignIn}
            activeOpacity={0.9}
            className="flex-row w-full items-center justify-center rounded-2xl h-14 bg-[#1544b2] shadow-lg shadow-blue-900/20"
          >
            <FontAwesome5 name="google" size={16} color="white" />
            <Text className="text-white text-base font-bold ml-3">
              Kontynuuj z Google
            </Text>
          </TouchableOpacity>

          <TouchableOpacity
            onPress={handleGuestSignIn}
            activeOpacity={0.7}
            className="flex-row w-full items-center justify-center rounded-2xl h-14 bg-white dark:bg-slate-800 border border-[#1544b2]/10 shadow-sm"
          >
            <MaterialIcons name="person-outline" size={20} color="#64748b" />
            <Text className="text-slate-600 dark:text-slate-300 text-sm font-semibold ml-2">
              Wypróbuj jako gość
            </Text>
          </TouchableOpacity>
        </View>

        <View className="mt-16 pt-8 border-t border-slate-200 dark:border-slate-800 w-full items-center">
          <Text className="text-[10px] text-slate-400 dark:text-slate-500 mb-6 uppercase tracking-[3px] font-bold">
            Inteligentny Asystent Kierowcy
          </Text>

          <View className="flex-row gap-3">
            <View className="w-2 h-2 rounded-full bg-[#1544b2]/40" />
            <View className="w-2 h-2 rounded-full bg-[#1544b2]/20" />
            <View className="w-2 h-2 rounded-full bg-[#1544b2]/10" />
          </View>
        </View>
      </View>
    </View>
  );
}
