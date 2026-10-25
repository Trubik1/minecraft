// ==============================================================================
//  KubeJS Server Script: Progression Cross-Mod Recipes for Minecraft 1.21.1
//  Сборка: Steampunk & Swords
//  Связка: Create ➔ Mekanism ➔ Ars Nouveau ➔ Refined Storage (МЭ-Сеть)
// ==============================================================================

ServerEvents.recipes(event => {
    console.info('=== Loading Steampunk & Swords Progression Recipes ===');

    // --------------------------------------------------------------------------
    // 1. MEKANISM: Металлургический наполнитель (Metallurgic Infuser)
    // Входной билет в Mekanism! Требует латунный корпус, пластины и электронную лампу Create.
    // --------------------------------------------------------------------------
    event.remove({ output: 'mekanism:metallurgic_infuser' });
    event.shaped('mekanism:metallurgic_infuser', [
        'SFS',
        'RCR',
        'PTP'
    ], {
        S: 'create:brass_sheet',
        F: 'minecraft:furnace',
        R: 'minecraft:redstone',
        C: 'create:brass_casing',
        P: 'create:copper_sheet',
        T: 'create:electron_tube'
    }).id('steampunk:progression/metallurgic_infuser');

    // --------------------------------------------------------------------------
    // 2. MEKANISM: Стальной корпус (Steel Casing)
    // Основа всех станков Mekanism. Укрепление латунного корпуса Create сталью и стеклом.
    // --------------------------------------------------------------------------
    event.remove({ output: 'mekanism:steel_casing' });
    event.shaped('mekanism:steel_casing', [
        'SGS',
        'GCG',
        'SGS'
    ], {
        S: 'mekanism:ingot_steel',
        G: 'minecraft:glass',
        C: 'create:brass_casing'
    }).id('steampunk:progression/steel_casing');

    // --------------------------------------------------------------------------
    // 3. MEKANISM: Базовая микросхема (Basic Control Circuit)
    // Требует электронную лампу Create, обогащенный сплав, редстоун и латунные дорожки.
    // --------------------------------------------------------------------------
    event.remove({ output: 'mekanism:basic_control_circuit' });
    event.shaped('mekanism:basic_control_circuit', [
        'SRS',
        'ATA',
        'SRS'
    ], {
        S: 'create:brass_sheet',
        R: 'minecraft:redstone',
        A: 'mekanism:alloy_infused',
        T: 'create:electron_tube'
    }).id('steampunk:progression/basic_control_circuit');

    // --------------------------------------------------------------------------
    // 4. ARS NOUVEAU: Аппарат зачарования (Enchanting Apparatus)
    // Магический алтарь. Требует латунный корпус Create, золотые пластины и самоцветы Источника.
    // --------------------------------------------------------------------------
    event.remove({ output: 'ars_nouveau:enchanting_apparatus' });
    event.shaped('ars_nouveau:enchanting_apparatus', [
        'GSG',
        'SCS',
        'GSG'
    ], {
        G: 'create:golden_sheet',
        S: 'ars_nouveau:source_gem',
        C: 'create:brass_casing'
    }).id('steampunk:progression/enchanting_apparatus');

    // --------------------------------------------------------------------------
    // 5. MEKANISM: Энергетический планшет (Energy Tablet)
    // Аккумулятор высокой емкости на Самоцвете Источника Ars Nouveau и сплавах Mekanism.
    // --------------------------------------------------------------------------
    event.remove({ output: 'mekanism:energy_tablet' });
    event.shaped('mekanism:energy_tablet', [
        'RGR',
        'ASA',
        'RGR'
    ], {
        R: 'minecraft:redstone',
        G: 'create:golden_sheet',
        A: 'mekanism:alloy_infused',
        S: 'ars_nouveau:source_gem'
    }).id('steampunk:progression/energy_tablet');

    // --------------------------------------------------------------------------
    // 6. REFINED STORAGE: Корпус механизма (Machine Casing)
    // Стойка серверов МЭ. Требует стальной корпус Mekanism, стальные слитки и латунь Create.
    // --------------------------------------------------------------------------
    event.remove({ output: 'refinedstorage:machine_casing' });
    event.shaped('refinedstorage:machine_casing', [
        'SBS',
        'BCB',
        'SBS'
    ], {
        S: 'mekanism:ingot_steel',
        B: 'create:brass_sheet',
        C: 'mekanism:steel_casing'
    }).id('steampunk:progression/rs_machine_casing');

    // --------------------------------------------------------------------------
    // 7. REFINED STORAGE: Контроллер МЭ-сети (Controller)
    // Венец технологий! Требует микросхему, энергопланшет, точный механизм Create и самоцвет Ars.
    // --------------------------------------------------------------------------
    event.remove({ output: 'refinedstorage:controller' });
    event.shaped('refinedstorage:controller', [
        'PCP',
        'MET',
        'PSP'
    ], {
        P: 'refinedstorage:advanced_processor',
        C: 'mekanism:basic_control_circuit',
        M: 'create:precision_mechanism',
        E: 'refinedstorage:machine_casing',
        T: 'mekanism:energy_tablet',
        S: 'ars_nouveau:source_gem'
    }).id('steampunk:progression/rs_controller');

    // --------------------------------------------------------------------------
    // 8. REFINED STORAGE: Дисковод МЭ-сети (Disk Drive)
    // Требует точный механизм Create, микросхему Mekanism, сундук, сталь и процессор RS.
    // --------------------------------------------------------------------------
    event.remove({ output: 'refinedstorage:disk_drive' });
    event.shaped('refinedstorage:disk_drive', [
        'SPS',
        'CEM',
        'STS'
    ], {
        S: 'mekanism:ingot_steel',
        P: 'refinedstorage:advanced_processor',
        C: 'mekanism:basic_control_circuit',
        E: 'refinedstorage:machine_casing',
        M: 'create:precision_mechanism',
        T: 'minecraft:chest'
    }).id('steampunk:progression/rs_disk_drive');

    // --------------------------------------------------------------------------
    // 9. REFINED STORAGE: Сетка крафта (Crafting Grid)
    // Терминал автоматической сборки. Требует сетку RS, точный механизм Create и самоцвет Ars.
    // --------------------------------------------------------------------------
    event.remove({ output: 'refinedstorage:crafting_grid' });
    event.shaped('refinedstorage:crafting_grid', [
        'BSB',
        'PGM',
        'BTB'
    ], {
        B: 'create:brass_sheet',
        S: 'ars_nouveau:source_gem',
        P: 'refinedstorage:advanced_processor',
        G: 'refinedstorage:grid',
        M: 'create:precision_mechanism',
        T: 'minecraft:crafting_table'
    }).id('steampunk:progression/rs_crafting_grid');

    console.info('=== Steampunk & Swords Progression Recipes Loaded Successfully! ===');

    // --------------------------------------------------------------------------
    // 10. MEKANISM METALLURGIC INFUSER: Электронная лампа Create (Electron Tube)
    // Массовое производство в Металлургическом наполнителе через насыщение редстоуном!
    // --------------------------------------------------------------------------

    // 1. Из железной пластины (create:iron_sheet) + 20 ед. редстоуна
    event.custom({
        type: 'mekanism:metallurgic_infusing',
        chemical_input: {
            amount: 20,
            tag: 'mekanism:redstone'
        },
        item_input: {
            count: 1,
            item: 'create:iron_sheet'
        },
        output: {
            count: 1,
            id: 'create:electron_tube'
        },
        per_tick_usage: false
    }).id('steampunk:infusing/electron_tube_from_iron_sheet');

    // 2. Из розового кварца (create:rose_quartz) + 20 ед. редстоуна (без полировки наждачкой!)
    event.custom({
        type: 'mekanism:metallurgic_infusing',
        chemical_input: {
            amount: 20,
            tag: 'mekanism:redstone'
        },
        item_input: {
            count: 1,
            item: 'create:rose_quartz'
        },
        output: {
            count: 1,
            id: 'create:electron_tube'
        },
        per_tick_usage: false
    }).id('steampunk:infusing/electron_tube_from_rose_quartz');

    // 3. Из полированного розового кварца (create:polished_rose_quartz) + 10 ед. редстоуна
    event.custom({
        type: 'mekanism:metallurgic_infusing',
        chemical_input: {
            amount: 10,
            tag: 'mekanism:redstone'
        },
        item_input: {
            count: 1,
            item: 'create:polished_rose_quartz'
        },
        output: {
            count: 1,
            id: 'create:electron_tube'
        },
        per_tick_usage: false
    }).id('steampunk:infusing/electron_tube_from_polished_quartz');

    // 4. Дополнительно: Ванильная Краснокаменная лампа (minecraft:redstone_lamp) из светящегося камня + 40 редстоуна
    event.custom({
        type: 'mekanism:metallurgic_infusing',
        chemical_input: {
            amount: 40,
            tag: 'mekanism:redstone'
        },
        item_input: {
            count: 1,
            item: 'minecraft:glowstone'
        },
        output: {
            count: 1,
            id: 'minecraft:redstone_lamp'
        },
        per_tick_usage: false
    }).id('steampunk:infusing/redstone_lamp_from_glowstone');


    // ==========================================================================
    // 11. ТЕХНОЛОГИЧЕСКИЕ И МАГИЧЕСКИЕ СВЯЗКИ (Mekanism x Create x Ars Nouveau)
    // ==========================================================================

    // 1. КОМБАЙНЕР MEKANISM: 100% сборка Точного механизма (БЕЗ РИСКА ПОЛОМКИ И БРАКА!)
    event.custom({
        type: 'mekanism:combining',
        main_input: {
            count: 1,
            item: 'create:brass_casing'
        },
        extra_input: {
            count: 1,
            item: 'create:electron_tube'
        },
        output: {
            count: 1,
            id: 'create:precision_mechanism'
        }
    }).id('steampunk:combining/precision_mechanism_100_percent');

    // 2. КОМБАЙНЕР MEKANISM: Сборка Точного механизма через микросхему Mekanism
    event.custom({
        type: 'mekanism:combining',
        main_input: {
            count: 1,
            item: 'create:brass_sheet'
        },
        extra_input: {
            count: 1,
            item: 'mekanism:basic_control_circuit'
        },
        output: {
            count: 1,
            id: 'create:precision_mechanism'
        }
    }).id('steampunk:combining/precision_mechanism_from_circuit');

    // 3. КАМЕРА ОБОГАЩЕНИЯ: Удвоение сырого цинка Create (x2 переработка руды)
    event.custom({
        type: 'mekanism:enriching',
        input: {
            count: 1,
            item: 'create:raw_zinc'
        },
        output: {
            count: 2,
            id: 'create:zinc_ingot'
        }
    }).id('steampunk:enriching/raw_zinc_doubling');

    // 4. КАМЕРА ОБОГАЩЕНИЯ: Умножение кристаллов маны (Source Gem)
    event.custom({
        type: 'mekanism:enriching',
        input: {
            count: 1,
            item: 'ars_nouveau:source_gem'
        },
        output: {
            count: 2,
            id: 'ars_nouveau:source_gem'
        }
    }).id('steampunk:enriching/source_gem_duplication');

    // 5. НАПОЛНИТЕЛЬ MEKANISM: Прямая штамповка латунных пластин Create из меди
    event.custom({
        type: 'mekanism:metallurgic_infusing',
        chemical_input: {
            amount: 10,
            tag: 'mekanism:redstone'
        },
        item_input: {
            count: 1,
            item: 'create:copper_sheet'
        },
        output: {
            count: 2,
            id: 'create:brass_sheet'
        },
        per_tick_usage: false
    }).id('steampunk:infusing/brass_sheet_from_copper');

    // ==========================================================================
    // 12. ЭПИЧЕСКИЙ БОЙ И ОРУЖИЕ БОССОВ (Cataclysm x Epic Fight x Create/Mekanism)
    // ==========================================================================

    // 1. ИНФЕРНАЛЬНЫЙ КИНЕТИЧЕСКИЙ МОЛОТ (Cataclysm Infernal Forge)
    // Объединение молота Монструозности, Парового двигателя Create и стали Mekanism
    event.remove({ output: 'cataclysm:infernal_forge' });
    event.shaped('cataclysm:infernal_forge', [
        'ABA',
        'CDC',
        ' E '
    ], {
        A: 'cataclysm:ignitium_ingot',
        B: 'create:steam_engine',
        C: 'mekanism:ingot_steel',
        D: 'cataclysm:monstrous_horn',
        E: 'epicfight:netherite_greatsword'
    }).id('steampunk:boss_craft/infernal_forge_upgraded');

    // 2. ПЛАЗМЕННЫЙ ИСПЕПЕЛИТЕЛЬ ИГНИСА (Cataclysm The Incinerator)
    // Великий меч Игниса, питаемый энергопланшетом Mekanism и кристаллами Ars Nouveau
    event.remove({ output: 'cataclysm:the_incinerator' });
    event.shaped('cataclysm:the_incinerator', [
        ' A ',
        'BCD',
        ' E '
    ], {
        A: 'cataclysm:ignitium_block',
        B: 'mekanism:energy_tablet',
        C: 'cataclysm:monstrous_horn',
        D: 'ars_nouveau:source_gem_block',
        E: 'epicfight:netherite_tachi'
    }).id('steampunk:boss_craft/the_incinerator_reforged');

    // 3. КНИГА БОЕВЫХ ИСКУССТВ (Epic Fight Master Skillbook)
    // Ковка фолианта боевых техник (парирование, уклонение, специальные стойки)
    event.shaped('epicfight:skillbook', [
        'ABA',
        'CDC',
        'ABA'
    ], {
        A: 'create:golden_sheet',
        B: 'ars_nouveau:source_gem',
        C: 'mekanism:alloy_infused',
        D: 'minecraft:book'
    }).id('steampunk:combat/master_skillbook');

    // 4. ТРОФЕЙНАЯ КАТАНА МАСТЕРА (Epic Fight Uchigatana)
    // Из стали Mekanism, Витерита Cataclysm и Точного механизма Create
    event.remove({ output: 'epicfight:uchigatana' });
    event.shaped('epicfight:uchigatana', [
        '  A',
        ' B ',
        'C  '
    ], {
        A: 'cataclysm:witherite_ingot',
        B: 'mekanism:ingot_steel',
        C: 'create:precision_mechanism'
    }).id('steampunk:combat/master_uchigatana');


    // ==========================================================================
    // 13. ЛЕСТНИЦА ИЗМЕРЕНИЙ (DivineRPG-Style Dimensional Ladder)
    // Покорение миров шаг за шагом: один мир открывает ключ к следующему!
    // ==========================================================================

    // 1. СТУПЕНЬ 2 ➔ 3: КАТАЛИЗАТОР ПОДЗЕМНОГО САДА (Undergarden Catalyst)
    // Требует: Огненный слиток Сумеречного Леса (с Гидры) + Латунь Create + Самоцвет Ars
    event.remove({ output: 'undergarden:catalyst' });
    event.shaped('undergarden:catalyst', [
        'ABA',
        'CDC',
        'AEA'
    ], {
        A: 'minecraft:gold_ingot',
        B: 'twilightforest:fiery_ingot',
        C: 'create:brass_sheet',
        D: 'ars_nouveau:source_gem',
        E: 'minecraft:diamond'
    }).id('steampunk:dimension/undergarden_catalyst');

    // 2. СТУПЕНЬ 3 ➔ 4: СВЯЩЕННЫЙ АЛТАРЬ РАЯ (The Aether Altar)
    // Требует: Забытый слиток Undergarden + Осколок Утровита + Сталь Mekanism + Блок Источника
    event.remove({ output: 'aether:altar' });
    event.shaped('aether:altar', [
        'AAA',
        'BCB',
        'DED'
    ], {
        A: 'aether:holystone',
        B: 'undergarden:utheric_shard',
        C: 'undergarden:forgotten_ingot',
        D: 'mekanism:ingot_steel',
        E: 'ars_nouveau:source_gem_block'
    }).id('steampunk:dimension/aether_altar_gated');

    // 3. СТУПЕНЬ 4 ➔ 5: СЕРДЦЕ ГЛУБИН ДЛЯ МИРА СКАЛКА (Deeper & Darker Heart of The Deep)
    // Требует: Слиток Игнития с босса Cataclysm + Рог Монструозности + Занорит Аэзера + Осколок Эха
    event.remove({ output: 'deeperdarker:heart_of_the_deep' });
    event.shaped('deeperdarker:heart_of_the_deep', [
        'ABA',
        'CDC',
        'AEA'
    ], {
        A: 'minecraft:echo_shard',
        B: 'cataclysm:ignitium_ingot',
        C: 'aether:zanite_gemstone',
        D: 'cataclysm:monstrous_horn',
        E: 'undergarden:forgotten_ingot'
    }).id('steampunk:dimension/heart_of_the_deep_gated');

    // 4. СТУПЕНЬ 5 ➔ 6: ОКО КРАЯ (Eye of Ender)
    // Требует Пыль Душ из измерения The Otherside (Deeper and Darker) для открытия Края!
    event.remove({ output: 'minecraft:ender_eye' });
    event.shaped('minecraft:ender_eye', [
        ' A ',
        'BCB',
        ' A '
    ], {
        A: 'deeperdarker:soul_dust',
        B: 'minecraft:blaze_powder',
        C: 'minecraft:ender_pearl'
    }).id('steampunk:dimension/ender_eye_gated');


    // ==========================================================================
    // СИСТЕМА УЛУЧШЕНИЯ РЮКЗАКОВ (LOLILAND STYLE)
    // Усложненная ступенчатая прогрессия рюкзаков и стаков
    // ==========================================================================

    // 1. Медный рюкзак: Базовый рюкзак + медные листы Create + медь
    event.remove({ output: 'sophisticatedbackpacks:copper_backpack' });
    event.shaped('sophisticatedbackpacks:copper_backpack', [
        'CSC',
        'SBS',
        'CSC'
    ], {
        C: 'minecraft:copper_ingot',
        S: 'create:copper_sheet',
        B: 'sophisticatedbackpacks:backpack'
    }).id('steampunk:backpack/copper_backpack_loliland');

    // 2. Железный рюкзак: Медный рюкзак + железные листы Create + слитки железа
    event.remove({ output: 'sophisticatedbackpacks:iron_backpack' });
    event.shaped('sophisticatedbackpacks:iron_backpack', [
        'ISI',
        'SCS',
        'ISI'
    ], {
        I: 'minecraft:iron_ingot',
        S: 'create:iron_sheet',
        C: 'sophisticatedbackpacks:copper_backpack'
    }).id('steampunk:backpack/iron_backpack_loliland');

    // 3. Золотой рюкзак: Железный рюкзак + золотые листы Create + сплав Mekanism + камень Источника
    event.remove({ output: 'sophisticatedbackpacks:gold_backpack' });
    event.shaped('sophisticatedbackpacks:gold_backpack', [
        'GSG',
        'AIA',
        'GSG'
    ], {
        G: 'minecraft:gold_ingot',
        S: 'create:golden_sheet',
        A: 'mekanism:alloy_infused',
        I: 'sophisticatedbackpacks:iron_backpack'
    }).id('steampunk:backpack/gold_backpack_loliland');

    // 4. Алмазный рюкзак: Золотой рюкзак + точный механизм Create + усиленный сплав Mekanism + алмазы
    event.remove({ output: 'sophisticatedbackpacks:diamond_backpack' });
    event.shaped('sophisticatedbackpacks:diamond_backpack', [
        'DMD',
        'AGA',
        'DMD'
    ], {
        D: 'minecraft:diamond',
        M: 'create:precision_mechanism',
        A: 'mekanism:alloy_reinforced',
        G: 'sophisticatedbackpacks:gold_backpack'
    }).id('steampunk:backpack/diamond_backpack_loliland');

    // 5. Незеритовый рюкзак: Алмазный рюкзак + атомный сплав Mekanism + незеритовые слитки / обломки
    event.remove({ output: 'sophisticatedbackpacks:netherite_backpack' });
    event.shaped('sophisticatedbackpacks:netherite_backpack', [
        'NAN',
        'ADA',
        'NAN'
    ], {
        N: 'minecraft:netherite_ingot',
        A: 'mekanism:alloy_atomic',
        D: 'sophisticatedbackpacks:diamond_backpack'
    }).id('steampunk:backpack/netherite_backpack_loliland');

    // 6. Шаблон улучшения рюкзака (Upgrade Base)
    // Требует латунные листы Create и кожу
    event.remove({ output: 'sophisticatedbackpacks:upgrade_base' });
    event.shaped('sophisticatedbackpacks:upgrade_base', [
        'SLS',
        'LIL',
        'SLS'
    ], {
        S: 'minecraft:string',
        L: 'minecraft:leather',
        I: 'create:brass_sheet'
    }).id('steampunk:backpack/upgrade_base_loliland');

    // 7. Улучшения стаков (Stack Upgrades I - IV)
    event.remove({ output: 'sophisticatedbackpacks:stack_upgrade_tier_1' });
    event.shaped('sophisticatedbackpacks:stack_upgrade_tier_1', [
        'SIS',
        'IUI',
        'SIS'
    ], {
        S: 'create:brass_sheet',
        I: 'minecraft:iron_block',
        U: 'sophisticatedbackpacks:upgrade_base'
    }).id('steampunk:backpack/stack_upgrade_t1_loliland');

    event.remove({ output: 'sophisticatedbackpacks:stack_upgrade_tier_2' });
    event.shaped('sophisticatedbackpacks:stack_upgrade_tier_2', [
        'GAG',
        'AUA',
        'GAG'
    ], {
        G: 'minecraft:gold_block',
        A: 'mekanism:alloy_infused',
        U: 'sophisticatedbackpacks:stack_upgrade_tier_1'
    }).id('steampunk:backpack/stack_upgrade_t2_loliland');

    event.remove({ output: 'sophisticatedbackpacks:stack_upgrade_tier_3' });
    event.shaped('sophisticatedbackpacks:stack_upgrade_tier_3', [
        'DAD',
        'AUA',
        'DAD'
    ], {
        D: 'minecraft:diamond_block',
        A: 'mekanism:alloy_reinforced',
        U: 'sophisticatedbackpacks:stack_upgrade_tier_2'
    }).id('steampunk:backpack/stack_upgrade_t3_loliland');

    event.remove({ output: 'sophisticatedbackpacks:stack_upgrade_tier_4' });
    event.shaped('sophisticatedbackpacks:stack_upgrade_tier_4', [
        'NAN',
        'AUA',
        'NAN'
    ], {
        N: 'minecraft:netherite_ingot',
        A: 'mekanism:alloy_atomic',
        U: 'sophisticatedbackpacks:stack_upgrade_tier_3'
    }).id('steampunk:backpack/stack_upgrade_t4_loliland');

    // ==========================================================================
    // СОЛНЕЧНЫЕ ПАНЕЛИ SOLAR FLUX REBORN (ИНТЕГРАЦИЯ CREATE, MEKANISM, TWILIGHT)
    // ==========================================================================

    // Солнечная панель 1: Зеркала + железные листы Create + редстоун
    event.remove({ output: 'solarflux:sp_1' });
    event.shaped('solarflux:sp_1', [
        'MMM',
        'RSR',
        'SSS'
    ], {
        M: 'solarflux:mirror',
        R: 'minecraft:redstone',
        S: 'create:iron_sheet'
    }).id('steampunk:solar/sp_1_craft');

    // Солнечная панель 2: 2x SP 1 + латунные листы Create + Фотоэлемент 1
    event.remove({ output: 'solarflux:sp_2' });
    event.shaped('solarflux:sp_2', [
        'PPP',
        'SCS',
        'SSS'
    ], {
        P: 'solarflux:sp_1',
        C: 'solarflux:photovoltaic_cell_1',
        S: 'create:brass_sheet'
    }).id('steampunk:solar/sp_2_craft');

    // Солнечная панель 3: 2x SP 2 + сплав Mekanism + Фотоэлемент 2
    event.remove({ output: 'solarflux:sp_3' });
    event.shaped('solarflux:sp_3', [
        'PPP',
        'ACA',
        'AAA'
    ], {
        P: 'solarflux:sp_2',
        C: 'solarflux:photovoltaic_cell_2',
        A: 'mekanism:alloy_infused'
    }).id('steampunk:solar/sp_3_craft');

    // Солнечная панель 4: 2x SP 3 + базовая схема Mekanism + Фотоэлемент 3
    event.remove({ output: 'solarflux:sp_4' });
    event.shaped('solarflux:sp_4', [
        'PPP',
        'ECE',
        'EEE'
    ], {
        P: 'solarflux:sp_3',
        C: 'solarflux:photovoltaic_cell_3',
        E: 'mekanism:basic_control_circuit'
    }).id('steampunk:solar/sp_4_craft');

    // Солнечная панель 5: 2x SP 4 + продвинутая схема Mekanism + Фотоэлемент 4
    event.remove({ output: 'solarflux:sp_5' });
    event.shaped('solarflux:sp_5', [
        'PPP',
        'ECE',
        'EEE'
    ], {
        P: 'solarflux:sp_4',
        C: 'solarflux:photovoltaic_cell_4',
        E: 'mekanism:advanced_control_circuit'
    }).id('steampunk:solar/sp_5_craft');

    // Солнечная панель 6: 2x SP 5 + элитная схема Mekanism + камень Источника + точный механизм
    event.remove({ output: 'solarflux:sp_6' });
    event.shaped('solarflux:sp_6', [
        'PPP',
        'ECE',
        'SGS'
    ], {
        P: 'solarflux:sp_5',
        C: 'solarflux:photovoltaic_cell_5',
        E: 'mekanism:elite_control_circuit',
        S: 'ars_nouveau:source_gem',
        G: 'create:precision_mechanism'
    }).id('steampunk:solar/sp_6_craft');

    // Солнечная панель 7: 2x SP 6 + абсолютная схема Mekanism + Звезда Незера
    event.remove({ output: 'solarflux:sp_7' });
    event.shaped('solarflux:sp_7', [
        'PPP',
        'ECE',
        'NSN'
    ], {
        P: 'solarflux:sp_6',
        C: 'solarflux:photovoltaic_cell_6',
        E: 'mekanism:ultimate_control_circuit',
        N: 'minecraft:nether_star',
        S: 'mekanism:alloy_atomic'
    }).id('steampunk:solar/sp_7_craft');

    // Солнечная панель 8 (Максимум): 2x SP 7 + точные механизмы + незеритовые блоки + антиматерия
    event.remove({ output: 'solarflux:sp_8' });
    event.shaped('solarflux:sp_8', [
        'PPP',
        'NAN',
        'MAM'
    ], {
        P: 'solarflux:sp_7',
        N: 'minecraft:netherite_block',
        A: 'mekanism:pellet_antimatter',
        M: 'create:precision_mechanism'
    }).id('steampunk:solar/sp_8_craft');

    // Огненная солнечная панель (Fiery Solar Panel): 4x SP 4 + огненные слитки Twilight Forest + жезлы блейза
    event.remove({ output: 'solarflux:sp_tf.fiery' });
    event.shaped('solarflux:sp_tf.fiery', [
        'FFF',
        'PSP',
        'PBP'
    ], {
        F: 'twilightforest:fiery_ingot',
        P: 'solarflux:sp_4',
        S: 'minecraft:blaze_powder',
        B: 'minecraft:blaze_rod'
    }).id('steampunk:solar/sp_fiery_craft');

    // Карминитовая солнечная панель (Carminite Solar Panel): 4x SP 5 + карминит Twilight Forest
    event.remove({ output: 'solarflux:sp_tf.carminite' });
    event.shaped('solarflux:sp_tf.carminite', [
        'CCC',
        'PSP',
        'PBP'
    ], {
        C: 'twilightforest:carminite',
        P: 'solarflux:sp_5',
        S: 'twilightforest:carminite_block',
        B: 'ars_nouveau:source_gem'
    }).id('steampunk:solar/sp_carminite_craft');

    // ==========================================================================
    // POWAH: ТЕХНО-МАГИЧЕСКАЯ ПРОГРЕССИЯ И КРОСС-МОДОВЫЕ РЕЦЕПТЫ
    // ==========================================================================

    // 1. Металлургический наполнитель: Массовое производство диэлектрической пасты через Углерод
    event.custom({
        type: 'mekanism:metallurgic_infusing',
        chemical_input: {
            amount: 20,
            tag: 'mekanism:carbon'
        },
        item_input: {
            count: 1,
            item: 'minecraft:clay_ball'
        },
        output: {
            count: 8,
            id: 'powah:dielectric_paste'
        },
        per_tick_usage: false
    }).id('steampunk:powah/infusing_dielectric_paste');

    // 2. Альтернативный крафт пасты диэлектрика через Create Cinder Flour
    event.shapeless('16x powah:dielectric_paste', [
        'create:cinder_flour',
        'minecraft:clay_ball',
        '#minecraft:coals'
    ]).id('steampunk:powah/dielectric_paste_cinder_flour');

    // 3. Диэлектрические корпуса с повышенным выходом через корпуса Create и Mekanism
    event.shaped('2x powah:dielectric_casing', [
        ' R ',
        'RCR',
        ' R '
    ], {
        R: 'powah:dielectric_rod',
        C: 'create:andesite_casing'
    }).id('steampunk:powah/dielectric_casing_andesite');

    event.shaped('4x powah:dielectric_casing', [
        ' R ',
        'RCR',
        ' R '
    ], {
        R: 'powah:dielectric_rod',
        C: 'create:brass_casing'
    }).id('steampunk:powah/dielectric_casing_brass');

    event.shaped('8x powah:dielectric_casing', [
        ' R ',
        'RCR',
        ' R '
    ], {
        R: 'powah:dielectric_rod',
        C: 'mekanism:steel_casing'
    }).id('steampunk:powah/dielectric_casing_steel');

    // 4. Зарядная сфера (Energizing Orb) — синтез Create, Mekanism и Ars Nouveau
    event.remove({ output: 'powah:energizing_orb' });
    event.shaped('powah:energizing_orb', [
        'RRR',
        'PCE',
        'RSR'
    ], {
        R: 'powah:dielectric_rod',
        P: 'create:precision_mechanism',
        C: 'powah:dielectric_casing',
        E: 'mekanism:basic_control_circuit',
        S: 'ars_nouveau:source_gem'
    }).id('steampunk:powah/energizing_orb_progression');

    // 5. Зарядоотводы (Energizing Rods) — ступенчатое технологическое улучшение
    // Базовый стержень: требует латунный лист Create
    event.remove({ output: 'powah:energizing_rod_basic' });
    event.shaped('powah:energizing_rod_basic', [
        ' C ',
        ' B ',
        ' S '
    ], {
        C: 'powah:capacitor_basic',
        B: 'create:brass_sheet',
        S: 'powah:energizing_rod_starter'
    }).id('steampunk:powah/energizing_rod_basic_craft');

    // Закаленный стержень: требует сталь под напряжением + обогащенный сплав Mekanism
    event.remove({ output: 'powah:energizing_rod_hardened' });
    event.shaped('powah:energizing_rod_hardened', [
        ' C ',
        ' A ',
        ' S '
    ], {
        C: 'powah:capacitor_hardened',
        A: 'mekanism:alloy_infused',
        S: 'powah:energizing_rod_basic'
    }).id('steampunk:powah/energizing_rod_hardened_craft');

    // Пылающий стержень: требует пылающий кристалл + армированный сплав Mekanism
    event.remove({ output: 'powah:energizing_rod_blazing' });
    event.shaped('powah:energizing_rod_blazing', [
        ' C ',
        ' A ',
        ' S '
    ], {
        C: 'powah:capacitor_blazing',
        A: 'mekanism:alloy_reinforced',
        S: 'powah:energizing_rod_hardened'
    }).id('steampunk:powah/energizing_rod_blazing_craft');

    // Ниотический стержень: требует ниотический кристалл + слиток очищенного обсидиана
    event.remove({ output: 'powah:energizing_rod_niotic' });
    event.shaped('powah:energizing_rod_niotic', [
        ' C ',
        ' A ',
        ' S '
    ], {
        C: 'powah:capacitor_niotic',
        A: 'mekanism:ingot_refined_obsidian',
        S: 'powah:energizing_rod_blazing'
    }).id('steampunk:powah/energizing_rod_niotic_craft');

    // Резонансный стержень: требует спиритический кристалл + атомарный сплав Mekanism
    event.remove({ output: 'powah:energizing_rod_spirited' });
    event.shaped('powah:energizing_rod_spirited', [
        ' C ',
        ' A ',
        ' S '
    ], {
        C: 'powah:capacitor_spirited',
        A: 'mekanism:alloy_atomic',
        S: 'powah:energizing_rod_niotic'
    }).id('steampunk:powah/energizing_rod_spirited_craft');

    // Нитро стержень: требует нитро-кристалл + точный механизм Create + звезда Незера
    event.remove({ output: 'powah:energizing_rod_nitro' });
    event.shaped('powah:energizing_rod_nitro', [
        ' C ',
        ' N ',
        ' S '
    ], {
        C: 'powah:capacitor_nitro',
        N: 'minecraft:nether_star',
        S: 'powah:energizing_rod_spirited'
    }).id('steampunk:powah/energizing_rod_nitro_craft');

    // 6. Уникальные рецепты Зарядной сферы (Energizing Orb Transmutation)
    // Create Андезитовый сплав + Золото -> 2x Сталь под напряжением
    event.custom({
        type: 'powah:energizing',
        energy: 20000,
        ingredients: [
            { item: 'create:andesite_alloy' },
            { item: 'minecraft:gold_ingot' }
        ],
        result: {
            count: 2,
            id: 'powah:steel_energized'
        }
    }).id('steampunk:powah/energizing_andesite_to_steel');

    // Сумеречный лес Сталелист + Золото -> 2x Сталь под напряжением (быстрая магическая проводка)
    event.custom({
        type: 'powah:energizing',
        energy: 15000,
        ingredients: [
            { item: 'twilightforest:steeleaf_ingot' },
            { item: 'minecraft:gold_ingot' }
        ],
        result: {
            count: 2,
            id: 'powah:steel_energized'
        }
    }).id('steampunk:powah/energizing_steeleaf_to_steel');

    // Mekanism Обогащенный алмаз -> Ниотический кристалл (скидка 50% энергии!)
    event.custom({
        type: 'powah:energizing',
        energy: 150000,
        ingredients: [
            { item: 'mekanism:enriched_diamond' }
        ],
        result: {
            count: 1,
            id: 'powah:crystal_niotic'
        }
    }).id('steampunk:powah/energizing_enriched_diamond_niotic');

    // Ars Nouveau Самоцвет источника + Порошок блейза -> Пылающий кристалл
    event.custom({
        type: 'powah:energizing',
        energy: 60000,
        ingredients: [
            { item: 'ars_nouveau:source_gem' },
            { item: 'minecraft:blaze_powder' }
        ],
        result: {
            count: 1,
            id: 'powah:crystal_blazing'
        }
    }).id('steampunk:powah/energizing_source_gem_blazing');

    // Эндер-ядро: 4x выход при использовании точного механизма Create!
    event.custom({
        type: 'powah:energizing',
        energy: 80000,
        ingredients: [
            { item: 'powah:dielectric_casing' },
            { item: 'minecraft:ender_eye' },
            { item: 'create:precision_mechanism' }
        ],
        result: {
            count: 4,
            id: 'powah:ender_core'
        }
    }).id('steampunk:powah/energizing_ender_core_quad');


    // ==========================================================================
    // ПРОГРЕССИЯ МИКРОСХЕМ MEKANISM: ПРОДВИНУТЫЕ, ЭЛИТНЫЕ (СИНИЕ) И СОВЕРШЕННЫЕ (ФИОЛЕТОВЫЕ)
    // ==========================================================================

    // --------------------------------------------------------------------------
    // 1. ПРОДВИНУТАЯ СХЕМА УПРАВЛЕНИЯ (Advanced Control Circuit - Красная)
    // Синтез: Базовая схема + Прецизионный механизм Create + Обогащенные сплавы + Самоцвет Источника + Полированный розовый кварц
    // --------------------------------------------------------------------------
    event.remove({ output: 'mekanism:advanced_control_circuit' });
    event.shaped('mekanism:advanced_control_circuit', [
        'QMQ',
        'ABA',
        'QSQ'
    ], {
        Q: 'create:polished_rose_quartz',
        M: 'create:precision_mechanism',
        A: 'mekanism:alloy_infused',
        B: 'mekanism:basic_control_circuit',
        S: 'ars_nouveau:source_gem'
    }).id('steampunk:circuits/advanced_control_circuit_shaped');

    // Промышленный метод в Наполнителе: Прецизионный механизм + 80 редстоуна
    event.remove({ id: 'mekanism:control_circuit/infused_advanced' });
    event.custom({
        type: 'mekanism:metallurgic_infusing',
        chemical_input: {
            amount: 80,
            tag: 'mekanism:redstone'
        },
        item_input: {
            count: 1,
            item: 'create:precision_mechanism'
        },
        output: {
            count: 1,
            id: 'mekanism:advanced_control_circuit'
        },
        per_tick_usage: false
    }).id('steampunk:circuits/advanced_from_precision_infusing');

    // --------------------------------------------------------------------------
    // 2. ЭЛИТНАЯ СХЕМА УПРАВЛЕНИЯ (Elite Control Circuit - СИНЯЯ)
    // Высокотехнологичная плата с охлаждением.
    // Требует: Продвинутая схема + Армированные сплавы + Ниотический кристалл Powah + Сапфировый Занит из Аэзера + Термостойкие листы Create
    // --------------------------------------------------------------------------
    event.remove({ output: 'mekanism:elite_control_circuit' });
    event.remove({ id: 'mekanism:control_circuit/infused_elite' });

    // Основной рецепт: через Небесный мир (The Aether)
    event.shaped('mekanism:elite_control_circuit', [
        'TNT',
        'ACA',
        'TZT'
    ], {
        T: 'create:sturdy_sheet',
        N: 'powah:crystal_niotic',
        A: 'mekanism:alloy_reinforced',
        C: 'mekanism:advanced_control_circuit',
        Z: 'aether:zanite_gemstone'
    }).id('steampunk:circuits/elite_control_circuit_aether');

    // Альтернативный рецепт: через Андергарден (Undergarden Морозная сталь)
    event.shaped('mekanism:elite_control_circuit', [
        'TNT',
        'ACA',
        'TFT'
    ], {
        T: 'create:sturdy_sheet',
        N: 'powah:crystal_niotic',
        A: 'mekanism:alloy_reinforced',
        C: 'mekanism:advanced_control_circuit',
        F: 'undergarden:froststeel_ingot'
    }).id('steampunk:circuits/elite_control_circuit_undergarden');

    // Металлургический наполнитель: Продвинутая схема + 160 ед. Алмазной пыли
    event.custom({
        type: 'mekanism:metallurgic_infusing',
        chemical_input: {
            amount: 160,
            tag: 'mekanism:diamond'
        },
        item_input: {
            count: 1,
            item: 'mekanism:advanced_control_circuit'
        },
        output: {
            count: 1,
            id: 'mekanism:elite_control_circuit'
        },
        per_tick_usage: false
    }).id('steampunk:circuits/elite_from_advanced_infusing');

    // --------------------------------------------------------------------------
    // 3. СОВЕРШЕННАЯ СХЕМА УПРАВЛЕНИЯ (Ultimate Control Circuit - ФИОЛЕТОВАЯ)
    // Квантово-магический венец вычислений.
    // Требует: Элитная (синяя) схема + Атомарные сплавы + Нитро-кристалл Powah + Карминит Сумеречного Леса + Очищенный обсидиан
    // --------------------------------------------------------------------------
    event.remove({ output: 'mekanism:ultimate_control_circuit' });
    event.remove({ id: 'mekanism:control_circuit/infused_ultimate' });

    // Основной рецепт: через Карминит Сумеречного Леса и Нитро-кристалл Powah
    event.shaped('mekanism:ultimate_control_circuit', [
        'ONO',
        'AEA',
        'OCO'
    ], {
        O: 'mekanism:ingot_refined_obsidian',
        N: 'powah:crystal_nitro',
        A: 'mekanism:alloy_atomic',
        E: 'mekanism:elite_control_circuit',
        C: 'twilightforest:carminite'
    }).id('steampunk:circuits/ultimate_control_circuit_tf');

    // Альтернативный рецепт: через Зачарованный гравитит Аэзера и Кристалл Утерия Андергардена
    event.shaped('mekanism:ultimate_control_circuit', [
        'GNG',
        'AEA',
        'GUG'
    ], {
        G: 'aether:enchanted_gravitite',
        N: 'powah:crystal_nitro',
        A: 'mekanism:alloy_atomic',
        E: 'mekanism:elite_control_circuit',
        U: 'undergarden:utherium_crystal'
    }).id('steampunk:circuits/ultimate_control_circuit_aether_undergarden');

    // Металлургический наполнитель: Элитная схема + 320 ед. Очищенного обсидиана
    event.custom({
        type: 'mekanism:metallurgic_infusing',
        chemical_input: {
            amount: 320,
            tag: 'mekanism:refined_obsidian'
        },
        item_input: {
            count: 1,
            item: 'mekanism:elite_control_circuit'
        },
        output: {
            count: 1,
            id: 'mekanism:ultimate_control_circuit'
        },
        per_tick_usage: false
    }).id('steampunk:circuits/ultimate_from_elite_infusing');

    // --------------------------------------------------------------------------
    // 4. МАССОВАЯ ЗАРЯДКА МИКРОСХЕМ В ЗАРЯДНОЙ СФЕРЕ POWAH (Двойной выход!)
    // --------------------------------------------------------------------------

    // Зарядка Продвинутой схемы (2x выход): Базовая схема + Точный механизм + Самоцвет Источника (100k FE)
    event.custom({
        type: 'powah:energizing',
        energy: 100000,
        ingredients: [
            { item: 'mekanism:basic_control_circuit' },
            { item: 'create:precision_mechanism' },
            { item: 'ars_nouveau:source_gem' }
        ],
        result: {
            count: 2,
            id: 'mekanism:advanced_control_circuit'
        }
    }).id('steampunk:circuits/energizing_advanced_double');

    // Зарядка Элитной схемы (Синей) (2x выход): Продвинутая схема + Ниотический кристалл + Занит (400k FE)
    event.custom({
        type: 'powah:energizing',
        energy: 400000,
        ingredients: [
            { item: 'mekanism:advanced_control_circuit' },
            { item: 'powah:crystal_niotic' },
            { item: 'aether:zanite_gemstone' }
        ],
        result: {
            count: 2,
            id: 'mekanism:elite_control_circuit'
        }
    }).id('steampunk:circuits/energizing_elite_double');

    // Зарядка Совершенной схемы (Фиолетовой) (2x выход): Элитная схема + Нитро-кристалл + Карминит (1.5M FE)
    event.custom({
        type: 'powah:energizing',
        energy: 1500000,
        ingredients: [
            { item: 'mekanism:elite_control_circuit' },
            { item: 'powah:crystal_nitro' },
            { item: 'twilightforest:carminite' }
        ],
        result: {
            count: 2,
            id: 'mekanism:ultimate_control_circuit'
        }
    }).id('steampunk:circuits/energizing_ultimate_double');

    // --------------------------------------------------------------------------
    // 5. REFINED STORAGE: ИНТЕГРАЦИЯ ПРОЦЕССОРОВ С МИКРОСХЕМАМИ MEKANISM
    // --------------------------------------------------------------------------

    // Улучшенный процессор (Синий RS): требует Базовую микросхему Mekanism + Золотой лист Create
    event.remove({ output: 'refinedstorage:raw_improved_processor' });
    event.shapeless('refinedstorage:raw_improved_processor', [
        'refinedstorage:processor_binding',
        'create:golden_sheet',
        'refinedstorage:silicon',
        'mekanism:basic_control_circuit'
    ]).id('steampunk:rs/raw_improved_processor_integrated');

    // Продвинутый процессор (Желтый RS): требует Продвинутую микросхему Mekanism + Алмаз + Кремний
    event.remove({ output: 'refinedstorage:raw_advanced_processor' });
    event.shapeless('refinedstorage:raw_advanced_processor', [
        'refinedstorage:processor_binding',
        'minecraft:diamond',
        'refinedstorage:silicon',
        'mekanism:advanced_control_circuit'
    ]).id('steampunk:rs/raw_advanced_processor_integrated');


    // ==========================================================================
    // EXTENDED CRAFTING: ЭНДГЕЙМ 9x9 МЕГА-КРАФТЫ (КРЕАТИВНЫЕ ПРЕДМЕТЫ)
    // ==========================================================================

    // 1. Креативный мотор Create (create:creative_motor)
    event.custom({
        type: 'extendedcrafting:shaped_table',
        pattern: [
            'MFPAUAPFM',
            'FSPCHCPFS',
            'PPKOTOKPP',
            'ACONBNOCA',
            'UHTBEBHTU',
            'ACONBNOCA',
            'PPKOTOKPP',
            'FSPCHCPFS',
            'MFPAUAPFM'
        ],
        key: {
            M: { item: 'create:brass_casing' },
            F: { item: 'create:flywheel' },
            P: { item: 'create:precision_mechanism' },
            A: { item: 'mekanism:alloy_atomic' },
            U: { item: 'mekanism:ultimate_control_circuit' },
            S: { item: 'create:speedometer' },
            C: { item: 'twilightforest:carminite' },
            H: { item: 'cataclysm:monstrous_horn' },
            K: { item: 'aether:enchanted_gravitite' },
            O: { item: 'mekanism:ingot_refined_obsidian' },
            T: { item: 'undergarden:forgotten_ingot' },
            N: { item: 'powah:crystal_nitro' },
            B: { item: 'minecraft:nether_star' },
            E: { item: 'minecraft:netherite_block' }
        },
        result: {
            id: 'create:creative_motor',
            count: 1
        }
    }).id('steampunk:endgame/creative_motor_9x9');

    // 2. Креативный энергетический куб Mekanism (mekanism:creative_energy_cube)
    event.custom({
        type: 'extendedcrafting:shaped_table',
        pattern: [
            'SCPUAUPFS',
            'CRUNTNURC',
            'PUMIEIMUP',
            'UNMOBOMNU',
            'ATEBKBETA',
            'UNMOBOMNU',
            'PUMIEIMUP',
            'CRUNTNURC',
            'SFPUAUPCS'
        ],
        key: {
            S: { item: 'solarflux:sp_8' },
            C: { item: 'solarflux:sp_tf.carminite' },
            F: { item: 'solarflux:sp_tf.fiery' },
            R: { item: 'powah:reactor_nitro' },
            P: { item: 'powah:energy_cell_nitro' },
            U: { item: 'mekanism:ultimate_control_circuit' },
            A: { item: 'mekanism:alloy_atomic' },
            N: { item: 'powah:crystal_nitro' },
            T: { item: 'mekanism:pellet_antimatter' },
            I: { item: 'mekanism:ultimate_induction_cell' },
            M: { item: 'mekanism:induction_casing' },
            O: { item: 'mekanism:ingot_refined_obsidian' },
            B: { item: 'minecraft:nether_star' },
            E: { item: 'mekanism:energy_cube' },
            K: { item: 'minecraft:netherite_block' }
        },
        result: {
            id: 'mekanism:creative_energy_cube',
            count: 1
        }
    }).id('steampunk:endgame/creative_energy_cube_9x9');

    // 3. Креативная энергоячейка Powah (powah:energy_cell_creative)
    event.custom({
        type: 'extendedcrafting:shaped_table',
        pattern: [
            'RRRRRRRRR',
            'RPUAIAUPR',
            'RUNTNTNUR',
            'RAIMEIMAR',
            'RUTBEBTUR',
            'RAIMEIMAR',
            'RUNTNTNUR',
            'RPUAIAUPR',
            'RRRRRRRRR'
        ],
        key: {
            R: { item: 'powah:dielectric_casing' },
            P: { item: 'powah:energy_cell_nitro' },
            U: { item: 'mekanism:ultimate_control_circuit' },
            A: { item: 'mekanism:alloy_atomic' },
            I: { item: 'powah:reactor_nitro' },
            N: { item: 'powah:crystal_nitro' },
            T: { item: 'mekanism:pellet_antimatter' },
            M: { item: 'powah:thermo_generator_nitro' },
            E: { item: 'solarflux:sp_8' },
            B: { item: 'minecraft:nether_star' }
        },
        result: {
            id: 'powah:energy_cell_creative',
            count: 1
        }
    }).id('steampunk:endgame/creative_energy_cell_powah_9x9');

    // 4. Креативный контроллер Refined Storage (refinedstorage:creative_controller)
    event.custom({
        type: 'extendedcrafting:shaped_table',
        pattern: [
            'PPPPUPPPP',
            'PCSAMASCP',
            'PSOTBTOSP',
            'PAEDRDEAP',
            'UMBKNKBME',
            'PAEDRDEAP',
            'PSOTBTOSP',
            'PCSAMASCP',
            'PPPPUPPPP'
        ],
        key: {
            P: { item: 'refinedstorage:advanced_processor' },
            U: { item: 'mekanism:ultimate_control_circuit' },
            C: { item: 'refinedstorage:controller' },
            S: { item: 'refinedstorage:64k_storage_part' },
            A: { item: 'mekanism:alloy_atomic' },
            M: { item: 'create:precision_mechanism' },
            O: { item: 'mekanism:ingot_refined_obsidian' },
            T: { item: 'ars_nouveau:archmage_spell_book' },
            B: { item: 'minecraft:nether_star' },
            E: { item: 'powah:crystal_nitro' },
            D: { item: 'refinedstorage:machine_casing' },
            R: { item: 'undergarden:forgotten_ingot' },
            K: { item: 'minecraft:netherite_block' },
            N: { item: 'refinedstorage:network_card' }
        },
        result: {
            id: 'refinedstorage:creative_controller',
            count: 1
        }
    }).id('steampunk:endgame/creative_controller_rs_9x9');

    // 5. Креативный диск памяти Refined Storage (refinedstorage:creative_storage_disk)
    event.custom({
        type: 'extendedcrafting:shaped_table',
        pattern: [
            'SSSSUSSSS',
            'SCPAPAPCS',
            'SPOMTMOSP',
            'SAENBNEAP',
            'UATKDKTAU',
            'PAENBNEAP',
            'SPOMTMOSP',
            'SCPAPAPCS',
            'SSSSUSSSS'
        ],
        key: {
            S: { item: 'refinedstorage:64k_storage_part' },
            U: { item: 'mekanism:ultimate_control_circuit' },
            C: { item: 'refinedstorage:creative_controller' },
            P: { item: 'refinedstorage:advanced_processor' },
            A: { item: 'mekanism:alloy_atomic' },
            O: { item: 'mekanism:ingot_refined_obsidian' },
            M: { item: 'create:precision_mechanism' },
            T: { item: 'powah:crystal_nitro' },
            E: { item: 'twilightforest:carminite' },
            N: { item: 'minecraft:nether_star' },
            B: { item: 'undergarden:forgotten_ingot' },
            K: { item: 'refinedstorage:storage_housing' },
            D: { item: 'minecraft:netherite_block' }
        },
        result: {
            id: 'refinedstorage:creative_storage_disk',
            count: 1
        }
    }).id('steampunk:endgame/creative_storage_disk_9x9');

});
