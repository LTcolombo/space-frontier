import { TokenMetadata } from "./metadata";
import { near } from "near-sdk-js";
export function internalAppendNumericValue({ contract, token_id, attribute, value }) {
    let metadata = contract.tokenMetadataById.get(token_id) as TokenMetadata;
    let extra = JSON.parse(metadata.extra);


    if (!extra.attributes)
        return;

    for (var trait of extra.attributes) {
        if (trait.trait_type == attribute) {
            trait.value = (trait.value || 0) + value;
            break;
        }
    }

    metadata.extra = JSON.stringify(extra);
    near.log(extra.attributes);
    contract.tokenMetadataById.set(token_id, metadata);
}