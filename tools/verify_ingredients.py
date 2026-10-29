import zipfile, glob

ingredients = [
    'cataclysm:ignitium_ingot',
    'cataclysm:void_core',
    'powah:crystal_nitro',
    'powah:ender_cell_nitro',
    'ars_nouveau:source_gem_block',
    'ars_nouveau:fire_essence',
    'ars_nouveau:water_essence',
    'ars_nouveau:earth_essence',
    'ars_nouveau:air_essence',
    'ars_nouveau:archmage_spell_book',
    'ars_nouveau:wilden_tribute',
    'ars_nouveau:source_jar',
    'create:blaze_cake',
    'create:blaze_cake_base',
    'create:precision_mechanism',
    'create:fluid_tank',
    'create:mechanical_pump',
    'create:brass_casing',
    'createaddition:alternator',
    'createaddition:electric_motor',
    'createaddition:tesla_coil',
    'twilightforest:carminite',
    'mekanism:pellet_antimatter',
    'mekanism:ultimate_control_circuit',
    'mekanism:ultimate_energy_cube',
    'mekanism:ultimate_fluid_tank',
    'mekanism:ultimate_chemical_tank',
    'mekanism:ultimate_bin',
    'industrialforegoing:plastic',
    'industrialforegoing:machine_frame_supreme',
    'refinedstorage:advanced_processor',
    'refinedstorage:storage_housing',
    'refinedstorage:quartz_enriched_iron_block',
    'refinedstorage:wireless_grid',
    'refinedstorage:wireless_autocrafting_monitor',
    'refinedstorage:portable_grid',
    'refinedstorage:range_upgrade',
    'pneumaticcraft:advanced_air_compressor',
    'pneumaticcraft:advanced_pressure_tube',
    'pneumaticcraft:pressure_chamber_wall',
    'pneumaticcraft:compressed_iron_block',
    'pneumaticcraft:upgrade_matrix',
    'functionalstorage:netherite_upgrade',
    'functionalstorage:diamond_upgrade',
    'storagedrawers:upgrade_template',
    'storagedrawers:emerald_storage_upgrade',
    'mysticalagradditions:insanium_block',
    'mysticalagriculture:awakened_supremium_block',
    'mysticalagriculture:awakened_supremium_ingot',
    'mysticalagriculture:master_infusion_crystal',
    'mysticalagriculture:soulium_dagger',
    'create_connected:fluid_vessel',
    'createbigcannons:autocannon_ammo_container'
]

all_items = set()
for jar in glob.glob("mods/*.jar"):
    try:
        with zipfile.ZipFile(jar, "r") as z:
            for name in z.namelist():
                if name.startswith("assets/") and "/models/item/" in name and name.endswith(".json"):
                    parts = name.split("/")
                    all_items.add(f"{parts[1]}:{parts[-1].replace('.json', '')}")
                elif name.startswith("data/") and "/items/" in name and name.endswith(".json"):
                    parts = name.split("/")
                    all_items.add(f"{parts[1]}:{parts[-1].replace('.json', '')}")
    except:
        pass

for ing in ingredients:
    if ing in all_items:
        print(f"OK: {ing}")
    else:
        print(f"MISSING: {ing}")
