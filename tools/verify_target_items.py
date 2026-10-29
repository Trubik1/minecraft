import subprocess

items_to_check = [
    # Ars Nouveau
    "ars_nouveau:creative_source_jar",
    "ars_nouveau:creative_spell_book",
    # Create & Addons
    "create:creative_fluid_tank",
    "create:creative_crate",
    "create_connected:creative_fluid_vessel",
    "createaddition:creative_energy",
    "createbigcannons:creative_autocannon_ammo_container",
    # Mekanism
    "mekanism:creative_chemical_tank",
    "mekanism:creative_bin",
    # PneumaticCraft
    "pneumaticcraft:creative_compressor",
    "pneumaticcraft:creative_compressed_iron_block",
    "pneumaticcraft:creative_upgrade",
    # Refined Storage & Addons
    "refinedstorage:creative_fluid_storage_disk",
    "refinedstorage:creative_fluid_storage_block",
    "refinedstorage:creative_storage_block",
    "refinedstorage:creative_wireless_grid",
    "refinedstorage:creative_wireless_autocrafting_monitor",
    "refinedstorage:creative_portable_grid",
    "refinedstorage:creative_range_upgrade",
    "refinedstorage_mekanism_integration:creative_chemical_storage_disk",
    "refinedstorage_mekanism_integration:creative_chemical_storage_block",
    "refinedstorage_quartz_arsenal:creative_wireless_crafting_grid",
    "universalgrid:creative_wireless_universal_grid",
    "cabletiers:creative_autocrafter",
    "cabletiers:creative_constructor",
    "cabletiers:creative_destructor",
    "cabletiers:creative_disk_interface",
    "cabletiers:creative_exporter",
    "cabletiers:creative_importer",
    "cabletiers:creative_interface",
    # Functional Storage & Storage Drawers
    "functionalstorage:creative_vending_upgrade",
    "storagedrawers:creative_vending_upgrade",
    "storagedrawers:creative_storage_upgrade",
    # Mystical Agriculture
    "mysticalagradditions:creative_essence",
    "mysticalagriculture:creative_soulium_dagger",
    # Titanium
    "titanium:creative_fe_generator"
]

out = subprocess.check_output(["python", "tools/find_creative_items.py"], text=True)
all_found = set(out.strip().splitlines())

for it in items_to_check:
    if it in all_found:
        print(f"OK: {it}")
    else:
        print(f"NOT FOUND: {it}")
