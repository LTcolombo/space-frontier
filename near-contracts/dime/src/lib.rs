/*!
Fungible Token implementation with JSON serialization.
NOTES:
  - The maximum balance value is limited by U128 (2**128 - 1).
  - JSON calls should pass U128 as a base-10 string. E.g. "100".
  - The contract optimizes the inner trie structure by hashing account IDs. It will prevent some
    abuse of deep tries. Shouldn't be an issue, once NEAR clients implement full hashing of keys.
  - The contract tracks the change in storage before and after the call. If the storage increases,
    the contract requires the caller of the contract to attach enough deposit to the function call
    to cover the storage cost.
    This is done to prevent a denial of service attack on the contract by taking all available storage.
    If the storage decreases, the contract will issue a refund for the cost of the released storage.
    The unused tokens from the attached deposit are also refunded, so it's safe to
    attach more deposit than required.
  - To prevent the deployed contract from being modified or deleted, it should not have any access
    keys on its account.
*/
use near_contract_standards::fungible_token::metadata::{
    FungibleTokenMetadata, FungibleTokenMetadataProvider, FT_METADATA_SPEC,
};
use near_contract_standards::fungible_token::FungibleToken;
use near_sdk::borsh::{self, BorshDeserialize, BorshSerialize};
use near_sdk::collections::LazyOption;
use near_sdk::json_types::U128;
use near_sdk::{env, log, near_bindgen, AccountId, Balance, PanicOnDefault, PromiseOrValue};

#[near_bindgen]
#[derive(BorshDeserialize, BorshSerialize, PanicOnDefault)]
pub struct Contract {
    token: FungibleToken,
    metadata: LazyOption<FungibleTokenMetadata>,
}

const DATA_IMAGE_SVG_NEAR_ICON: &str = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAAEEfUpiAAAAAXNSR0IArs4c6QAAAIRlWElmTU0AKgAAAAgABQESAAMAAAABAAEAAAEaAAUAAAABAAAASgEbAAUAAAABAAAAUgEoAAMAAAABAAIAAIdpAAQAAAABAAAAWgAAAAAAAAEsAAAAAQAAASwAAAABAAOgAQADAAAAAQABAACgAgAEAAAAAQAAACCgAwAEAAAAAQAAACAAAAAAOV9/2wAAAAlwSFlzAAAuIwAALiMBeKU/dgAAAp1pVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IlhNUCBDb3JlIDYuMC4wIj4KICAgPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICAgICAgPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIj4KICAgICAgICAgPHRpZmY6WVJlc29sdXRpb24+MzAwPC90aWZmOllSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpSZXNvbHV0aW9uVW5pdD4yPC90aWZmOlJlc29sdXRpb25Vbml0PgogICAgICAgICA8dGlmZjpYUmVzb2x1dGlvbj4zMDA8L3RpZmY6WFJlc29sdXRpb24+CiAgICAgICAgIDx0aWZmOk9yaWVudGF0aW9uPjE8L3RpZmY6T3JpZW50YXRpb24+CiAgICAgICAgIDxleGlmOlBpeGVsWERpbWVuc2lvbj4xMDA8L2V4aWY6UGl4ZWxYRGltZW5zaW9uPgogICAgICAgICA8ZXhpZjpQaXhlbFlEaW1lbnNpb24+OTQ8L2V4aWY6UGl4ZWxZRGltZW5zaW9uPgogICAgICA8L3JkZjpEZXNjcmlwdGlvbj4KICAgPC9yZGY6UkRGPgo8L3g6eG1wbWV0YT4KAG8u6AAADFlJREFUWAmdV3lcVNe9/947dxYGmBlgZoCBgWFHAVlUMLhvLW5ZJGjSuiTtS+J7MUmjbW1eX1JqWts0VVO7GG1j0uircWmiqGjcQRERnAg4yCLrsM0wMzALs8+c3hkD0by0n37e+eOcc+/5Ld/f7/x+v3MO8GUjhNCBKRXopL/aoHW5hfGKl54+FfhG9JFfEAPxkZjGSw/IymeXQBovx7j7RpDga11lJV+xfSM5QQb9L1jupGPa5d9cn+UaJKvG1AQN722lUP5dUbzJYR4rzoPtmbeCWidkUN/R9+fhwIHkb98/tWHiJ8oDAKveXiT/z1WE9+xjpOjCXiJ9fCHZTSxkXUNjAV1OUf4kTqg05ollCGfiYahphyw+AW//bgcUEa7cSR2v9ldvr+kc6rxt6N6MONnrRWlZprqoWS2TqkDIJPFXP4GggwKLrwzWlT8+WLcTjWeKbnpsu1mirxiKWs4vF7z9I8+aGweJcv8fyU0W4IahtmWshyl652hbrsk7ciZn3Mmc/+AD2C+ewQ63AXExvEpWBMXIzOMzOrpGiNDhpP68/zQiaA6W/3wrtP+1EESmItRMzSffjQxLOaSz90J46CJclY1w/3Qthudl4hk6poiu59UczzcPvnUnnk+clAXNi6Vo7u/yZZokT/5emndrEmns7YNzBW0Du5WZCtvMmGm1OxV5bzxs7uQ86HoyKFxw6rnMtWQ0t2p88H+mn1IIJwm+NpnUEHTksWOhkeK2kogwxVxhY+erjngavHA/6IwM3B3z+H8dlvW5m8/jHNM1/LA050VNYBsn5T3fVrVR8d5r56gjv3RDDrL39BEiX1pIRD9ZS0IXzSd4dwtRExfrY6t/2ZB6q6Lxk4IAM3WUEM6BvqrV59qO56Cq56dwq2g4fJguEsG1dTOeFItgG2vFie5mcP5+C0nbtiFbIsB7N28thNZzjVlDUb4VfVcS4pKzhAPyhSR5sAddXAnsGi9y+CFYzwlHQ1Qezg13orVajZ7o93E33woltZi3bWF2bjBW6vR3ruj6hX9Ht8ZVMC2DFOl7YbpxFFUbS5GRLkLF+bMY3n8GcI7AFz3udaSVXU5LkXs3y6aqv3Iia09u26dbZGZnf21985HxnEKkqZv84akU4YFPNdVcou3PF+inVRXPaoq7MIjlv3ezLN8cwgvIFYa57Q29OMRwQ9DHd3ivyNZnPe46mL76XnC3KIoEHPiv2iPI/hXhg3z5GsWMloNLqt2WFxrtxvjZXZ8vnVgOBtnEx5fjIwLKNEd552yDf2sQFxjmjdzv/sRq/E5Ns7M7p+NkWeGI+tlSj71goopNyGEmJlkaDe+eoTnlmLSXLzl5+IQqJjrhfWM7CkrmvqP28UCZ7F/UuyjNn2CxszwvTfAFEewjhBtmvTNba+nNqqu4sHoKVxp/t+Is4pTT0binEd9zhOFk2pIcKi52HbHFfbzFpJlMJDqQ9mn67ql1YSEmjzT72NCv30KpO4L2iqS4u+JVUNYh1P73IRjtdmaqL5TMyhBf77fx1ChDSAAFTbFb8rGjv1Raf3VfzOmPfMaUhdh9X42Zo+N4rb0Golwleh0jeF59GTuMfVQKhH493TYjY6c6Iigg0NloJsxzs6moSxzL4WbmIk+hhGvFc1AmF+HFTT+G/Z3ngHc/QH1DM1r9LkwRhnwvwyzxBQX0+5ybjnuNbDVRIuX7JfDcuoyamfMw7an5WM1SbIYMJVMWYeo7r8BWW4tTtJXqol3y27rPpgbM5+xaGHVf5aFj7XEpK5fkytFsckJg98NmIuClKBBKMejRdUJTpYap4Qu08keJN5ZYS3kpumXStKs02l8fl/OEVofAauTaPGT6hqUIb7gE44E/4eQHf8AL84px7ce74DH2IvnpJ4Cq66TXGlmtEsulQRPYHfU02VGDUNdBrSWCMoQBvhEbdDWX0NmigaBkCTyxEfDeH0DHtpfAfH8VZ64nfVePw/LmAwFsn+5u9l3NfNJ9+dybt7aETfEod7wCWC0YvlSFls/Po1fB1pj8JNDbf0DksY/tDAu32AdUI9aAgK+SppzQb2ys3v2rj9+fguXLl84xcn3hRgdFy0bJmZt1lGq+gu5JTH4Z7fYT+Na2wQBzoE0KCCSKxdCYuuteVcGK5Pm6MzUnnxJ5+SW8aL4uIj7qlGog/E7KNJXMjZCKA7LMoPYHIh7qy0n5ZHKVNVxPQGWlDBVvJ6VUb58dUFZGjnKC9eAhnn9r+kgatx5WsEyCH5AhFYg2MiAgtXIPH1cWPEhMNjbKjrKK/s026YJvog8EWiBVdpm1ke0e/dbGke7q/1AuJGn8kJJ5HecOnFUV7wgndN9yMn56NkfgODvQde95YiYKWXKi3jzo+HPionusXH/Q6n9SBb8ZQDnryrKpDHrzqa0xfTF5KtX+7ZbRPR36PhsbHTNTJZFrQ/mh8YODeotodEw84B+zpEaLHW5QegklP3JLa6o5VbJolkgkXnnXbal9WXviALK2mMrYI+wYewo9bOzXAVBoaGBg7eIXi1Bo9bpEfS5teqInfGtTRYUc1y9A/thinyA9FqNNg7TV1+PPWL0OAokMjFSGosw8KsHH0CBOnNV3kiqrbV+BUXosOYafKA7hpfloHP4oprgZhA0+iu3ZNgkgECDHqDU+3NibP0+coRwwtNmGD539iz1vcRI3xOGNGuhmhnh2TKsbwjiXhjWaDxdri9toBpEoESKLw6hMgCXz0sDLzMBWfgYpQLj/AoycX462H1naH/c7bdSovMV4r01PvFpd7obxgH4W7YOTrVCXIfjfkfbtq4VKSXX9X9M6Gfkl58anEzmfvkbcpysZYnDjwznPISc7G50yBjnLVyM/ewYcKikooR+umlpEn76KmorbmKNngXk91BsuLacfPt+HEUVr/WnWv5gtbqciMj1Wx+eKv9wGQgXOGDbQ/J+MdZU1jQ5M39Ff2RbW0LcnR5EU1lNRT3QQU/4xOxiFCGKzBTzaBc+oBYbcJGx6+ScIkUbgVNs1eL9og/P+fYxHSqDILgSPz0ez1Y+dyx/HCkbgOwgrRzNy4yzdEf2zMIZ0fVxYaAIbmExAOQtC+amps+gax+NWmZ3Lei91htXGuvypm9fT8iEThiubIYjhQ5Q6HfpvzcD63KlIpiRYwJohhA8r81dDl+/HdqcGEm0XSKcBrSMjUDAy7NW2oDUhjurxDkIqEBUdxpXsHHeWsRwYRXk5FczXkE1PKV/UterDaV+qT6RUWjySNOkTRUQwK4bqu3sfPLUGHPYmHpKdhkRaAjlPjCQhD7EsAAXhgB+MKQ5EXClUUUo0tGlgvt4GD+utcYsTdjmDgZ4LlDkcXr0l9PNQxjT1hL3t3tXSV53B4nF7X17H4nWnVhgs7rshNrXWVzx3mSpBBPn4KPr1ZhSsXwlHkhi0ugEWjRmNiTLoZTJcSVQhWZmALp0WNzrvQXD7DuzspUc0pwD6hutw9idC8IwY9z7a4xcuyuL02bjtpf4pTZunyN410f7WUuBi0AMtMvZdVLxSQJxerUsSHzU2VCNyemTJqTGRvlBVDN2WJoW3ug6t5R/CyYxj+GIVvLphjJgMaPzj+zAN6MHU34Xh9Hm4baMw1l4HSVBA4PXCcWMf/KXrKVFKLjXXn/gbm3tAf5zo2t0+uvH2zv3mAAAKLfD3ryvwxQgYKqFf0LQpd478i/bPLLUcaeabKYVel99E148aIRaEwVrNBhwTCmOvFobhPtj7tRhWX0Q46w1iJzBcU8PrcYAYhuEpmenHosVUeLSYcGUFG2JtZlN/lqqhJWJ2/arfKqxXf3710ctloOaX/5YKeX3x0RnjYaHP7h/W3lnV0vVi2pKl+WcUXFIckuS729PKxN5QY7SjCdeO10GeFwuDwwp/ynTEFmTAYhgkbk2v158j54bOk4EbP38v6Y44FBGut3SmrtSwBj9yMZ0sROzCgxY40ahyP5r/Fn0oNn8X30fryjpOflbYTc281df6FBMS8pjXHcWBUIIUDoNOBcGsxAiMCUfQyvFAweGahrmOw0n6qGtzeemhH403NxZKYk23QjEM5RrHhJqJ8f8C+HIl6I3AI6ZjjyjJr8zs7mmxr4nOlsmlCVInl+G323pIt3nIauCGG0KZSNdiviRZzDBp494xYbiAz16F6IN/UBS3Tij6/48PKmUQaOB0nBTElu4FV64wbCWly9lxD/tan1xjJwEDHv7+Z/N/ABP+qFv8j9nuAAAAAElFTkSuQmCC";

#[near_bindgen]
impl Contract {
    /// Initializes the contract with the given total supply owned by the given `owner_id` with
    /// default metadata (for example purposes only).
    #[init]
    pub fn new_default_meta(owner_id: AccountId, total_supply: U128) -> Self {
        Self::new(
            owner_id,
            total_supply,
            FungibleTokenMetadata {
                spec: FT_METADATA_SPEC.to_string(),
                name: "Frontier Dime".to_string(),
                symbol: "DIME".to_string(),
                icon: Some(DATA_IMAGE_SVG_NEAR_ICON.to_string()),
                reference: None,
                reference_hash: None,
                decimals: 18,
            },
        )
    }

    /// Initializes the contract with the given total supply owned by the given `owner_id` with
    /// the given fungible token metadata.
    #[init]
    pub fn new(
        owner_id: AccountId,
        total_supply: U128,
        metadata: FungibleTokenMetadata,
    ) -> Self {
        assert!(!env::state_exists(), "Already initialized");
        metadata.assert_valid();
        let mut this = Self {
            token: FungibleToken::new(b"a".to_vec()),
            metadata: LazyOption::new(b"m".to_vec(), Some(&metadata)),
        };
        this.token.internal_register_account(&owner_id);
        this.token.internal_deposit(&owner_id, total_supply.into());
        near_contract_standards::fungible_token::events::FtMint {
            owner_id: &owner_id,
            amount: &total_supply,
            memo: Some("Initial tokens supply is minted"),
        }
        .emit();
        this
    }

    fn on_account_closed(&mut self, account_id: AccountId, balance: Balance) {
        log!("Closed @{} with {}", account_id, balance);
    }

    fn on_tokens_burned(&mut self, account_id: AccountId, amount: Balance) {
        log!("Account @{} burned {}", account_id, amount);
    }
}

near_contract_standards::impl_fungible_token_core!(Contract, token, on_tokens_burned);
near_contract_standards::impl_fungible_token_storage!(Contract, token, on_account_closed);

#[near_bindgen]
impl FungibleTokenMetadataProvider for Contract {
    fn ft_metadata(&self) -> FungibleTokenMetadata {
        self.metadata.get().unwrap()
    }
}

#[cfg(all(test, not(target_arch = "wasm32")))]
mod tests {
    use near_sdk::test_utils::{accounts, VMContextBuilder};
    use near_sdk::MockedBlockchain;
    use near_sdk::{testing_env, Balance};

    use super::*;

    const TOTAL_SUPPLY: Balance = 1_000_000_000_000_000;

    fn get_context(predecessor_account_id: AccountId) -> VMContextBuilder {
        let mut builder = VMContextBuilder::new();
        builder
            .current_account_id(accounts(0))
            .signer_account_id(predecessor_account_id.clone())
            .predecessor_account_id(predecessor_account_id);
        builder
    }

    #[test]
    fn test_new() {
        let mut context = get_context(accounts(1));
        testing_env!(context.build());
        let contract = Contract::new_default_meta(accounts(1).into(), TOTAL_SUPPLY.into());
        testing_env!(context.is_view(true).build());
        assert_eq!(contract.ft_total_supply().0, TOTAL_SUPPLY);
        assert_eq!(contract.ft_balance_of(accounts(1)).0, TOTAL_SUPPLY);
    }

    #[test]
    #[should_panic(expected = "The contract is not initialized")]
    fn test_default() {
        let context = get_context(accounts(1));
        testing_env!(context.build());
        let _contract = Contract::default();
    }

    #[test]
    fn test_transfer() {
        let mut context = get_context(accounts(2));
        testing_env!(context.build());
        let mut contract = Contract::new_default_meta(accounts(2).into(), TOTAL_SUPPLY.into());
        testing_env!(context
            .storage_usage(env::storage_usage())
            .attached_deposit(contract.storage_balance_bounds().min.into())
            .predecessor_account_id(accounts(1))
            .build());
        // Paying for account registration, aka storage deposit
        contract.storage_deposit(None, None);

        testing_env!(context
            .storage_usage(env::storage_usage())
            .attached_deposit(1)
            .predecessor_account_id(accounts(2))
            .build());
        let transfer_amount = TOTAL_SUPPLY / 3;
        contract.ft_transfer(accounts(1), transfer_amount.into(), None);

        testing_env!(context
            .storage_usage(env::storage_usage())
            .account_balance(env::account_balance())
            .is_view(true)
            .attached_deposit(0)
            .build());
        assert_eq!(contract.ft_balance_of(accounts(2)).0, (TOTAL_SUPPLY - transfer_amount));
        assert_eq!(contract.ft_balance_of(accounts(1)).0, transfer_amount);
    }
}