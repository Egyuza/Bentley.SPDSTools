=============================  Bentley.SPDSTools  ==============================
            Bentley Microstation-Based Addin for creating SPDS tools
                    2018 ©  Ирисофт.  All rights reserved.
================================================================================

------------------------------ СОСТАВ ДИСТРИБУТИВА -----------------------------

  • SPDSTools.dll - Addin(.NET) для платформы Microstation
  • SPDSTools.01.00.ecschema.xml - EC-схема атрибутов SPDS-элементов;
  • папки с библиотеками локализаций ("\en", "\ru")


---------------------------------- УСТАНОВКА -----------------------------------

1. SPDSTools.dll и папки локализации скопировать в каталог надстроек программы 
   Bentley на базе Microstation: "<ProgramPath>\Mdlapps\"
2. SPDSTools.01.00.ecschema.xml схему скопировать в каталог 
   "<ProgramPath>\ECSchemas\Dgn\" для автоматического чтения при загрузке


------------------------------------ ЗАПУСК ------------------------------------
• АВТОЗАГРУЗКА : переменная конфигурации "MS_DGNAPPS = SPDSTools.dll"
• KEY-IN: "TOOLS START HEIGHT"


--------------------------- ПЕРЕМЕННЫЕ КОНФИГУРАЦИИ ----------------------------
HEIGHT -"ВЫСОТНАЯ ОТМЕТКА"
Common:
    AEP_SPDS_HEIGHT_STYLE [string] - СТИЛЬ тула, определяющий другие параметры
        значения: 'Default', 'Paks', 'ED'
    AEP_SPDS_HEIGHT_TEXTSTYLE [string]- стиль текста
    AEP_SPDS_HEIGHT_TEXTSTYLE_EDIT [boolean]- возм. задания через форму
    AEP_SPDS_HEIGHT_AUTOCALC [boolean] - авт. расчёт
    AEP_SPDS_HEIGHT_CALCBYITEM [boolean] - авт. расчёт по компоненту

стиль Default:
    AEP_SPDS_HEIGHT_DEFAULT_GAP_START [double] - отступ от точки указателя
    AEP_SPDS_HEIGHT_DEFAULT_GAP_ARROW_AFTER [double] - отступ после стрелки
    AEP_SPDS_HEIGHT_DEFAULT_GAP_TEXT_BEFORE [double] - отступ перед текстом
    AEP_SPDS_HEIGHT_DEFAULT_GAP_TEXT_AFTER [double] - отступ после текста
    AEP_SPDS_HEIGHT_DEFAULT_GAP_TEXT_BOTTOM [double] - отступ до текста снизу
    AEP_SPDS_HEIGHT_DEFAULT_GAP_TEXT_TOP [double] - отступ до текста сверху
    AEP_SPDS_HEIGHT_DEFAULT_LENGTH_BEFORE_ARROW [double] - базовая длина до стрелки
    AEP_SPDS_HEIGHT_DEFAULT_ARROW_DX [double] - половина ширины указателя стелки
    AEP_SPDS_HEIGHT_DEFAULT_ARROW_DY [double] - высота указателя стрелки
    AEP_SPDS_HEIGHT_DEFAULT_LANDING_HEIGHT [double] - высота полки
    AEP_SPDS_HEIGHT_DEFAULT_ZEROPREFIX [string] - префикс нулевой отметки
    AEP_SPDS_HEIGHT_DEFAULT_DECIMALSEPARATOR [string] - разделитель дробной части
    AEP_SPDS_HEIGHT_DEFAULT_PLAN_POINTFILLCOLOR [uint] - цвет заливки точки плановой выносной отметки
    AEP_SPDS_HEIGHT_DEFAULT_PLAN_POINTRADIUS [double] - радиус точки указателя

стиль Paks:
    AEP_SPDS_HEIGHT_PAKS_LINESTYLE [int] - стиль выносной линии
    AEP_SPDS_HEIGHT_PAKS_ARROW_RADIUS [double] - радиус окружности указателя
    AEP_SPDS_HEIGHT_PAKS_ARROW_RADIUS2 [double] - радиус окружности указателя
    AEP_SPDS_HEIGHT_PAKS_ARROW_FILLCOLOR [uint] - цвет заливки указателя
    AEP_SPDS_HEIGHT_PAKS_ARROW_DX [double] - половина ширины указателя стелки
    AEP_SPDS_HEIGHT_PAKS_ARROW_DY [double] - высота указателя стрелки
    AEP_SPDS_HEIGHT_PAKS_LANDING_LENGTH [double] - базовая длина
    AEP_SPDS_HEIGHT_PAKS_GAP_TEXT_BEFORE [double] - отступ перед текстом
    AEP_SPDS_HEIGHT_PAKS_GAP_TEXT_AFTER [double] - отступ после текста
    AEP_SPDS_HEIGHT_PAKS_GAP_TEXT_BOTTOM [double] - отступ до текста снизу
    AEP_SPDS_HEIGHT_PAKS_GAP_TEXT_TOP [double] - отступ до текста сверху
    AEP_SPDS_HEIGHT_PAKS_ZEROPREFIX [string] - префикс нулевой отметки
    AEP_SPDS_HEIGHT_PAKS_DECIMALSEPARATOR [string] - разделитель дробной части

стиль ED (El-Daabe):
    наследуются настройки стиля "Paks"