(() => {
	"use strict";

	const places = [
		{ id: "nbo", name: "Nairobi, Kenya", lat: -1.2864, lng: 36.8172 },
		{ id: "msa", name: "Mombasa, Kenya", lat: -4.0435, lng: 39.6682 },
		{ id: "nkr", name: "Nakuru, Kenya", lat: -0.3031, lng: 36.0800 },
		{ id: "kis", name: "Kisumu, Kenya", lat: -0.0917, lng: 34.7680 },
		{ id: "eld", name: "Eldoret, Kenya", lat: 0.5143, lng: 35.2698 },
		{ id: "thk", name: "Thika, Kenya", lat: -1.0333, lng: 37.0693 }
	];

	// Demo fixtures model vehicles returning from their loaded trip.
	const trucks = [
		{ id: 21, plate: "KDA 482R", type: "10-ton curtain-side", currentFrom: "Mombasa, Kenya", currentTo: "Nairobi, Kenya", startsInHours: 2, returnInHours: 11, returnWindowHours: 36, agent: "Miriam Wanjiku", vehicleOwner: "Coastline Hauliers" },
		{ id: 22, plate: "KCY 917M", type: "7-ton box truck", currentFrom: "Mombasa, Kenya", currentTo: "Nairobi, Kenya", startsInHours: 5, returnInHours: 14, returnWindowHours: 48, agent: "Peter Otieno", vehicleOwner: "Pioneer Transport" },
		{ id: 23, plate: "KDG 330A", type: "5-ton flatbed", currentFrom: "Nakuru, Kenya", currentTo: "Nairobi, Kenya", startsInHours: 1, returnInHours: 6, returnWindowHours: 24, agent: "Asha Njeri", vehicleOwner: "Highland Freight" },
		{ id: 24, plate: "KDM 604P", type: "12-ton truck", currentFrom: "Kisumu, Kenya", currentTo: "Nairobi, Kenya", startsInHours: 7, returnInHours: 18, returnWindowHours: 36, agent: "Daniel Kiptoo", vehicleOwner: "Lake Routes Ltd" },
		{ id: 25, plate: "KDL 581T", type: "8-ton refrigerated", currentFrom: "Nairobi, Kenya", currentTo: "Mombasa, Kenya", startsInHours: 4, returnInHours: 13, returnWindowHours: 36, agent: "Faith Kamau", vehicleOwner: "Metro Logistics" }
	];

	const state = {
		stage: "post",
		cargo: null,
		availableTrucks: [],
		selectedTruckId: null,
		bid: null,
		receipt: null,
		activity: [],
		receiptObjectUrl: null
	};

	const $ = (selector) => document.querySelector(selector);
	const $$ = (selector) => [...document.querySelectorAll(selector)];
	const form = $("#cargoForm");

	function bindPublicAuth() {
		const overlay = $("#authOverlay");
		const authForm = $("#authForm");
		if (!overlay || !authForm) return;

		const registerFields = $("#authRegisterFields");
		const roleSelect = $("#authRole");
		const feedback = $("#authFeedback");
		let mode = "register";

		function updateRegistrationRoleFields() {
			const role = roleSelect.value;
			const ownerFields = $("#vehicleOwnerFields");
			const driverFields = $("#driverFields");
			ownerFields.classList.toggle("hidden", role !== "VehicleOwner");
			driverFields.classList.toggle("hidden", role !== "Driver");
			ownerFields.querySelectorAll("input").forEach((input) => { input.disabled = role !== "VehicleOwner"; });
			driverFields.querySelectorAll("input").forEach((input) => { input.disabled = role !== "Driver"; });
		}

		function renderAuthMode() {
			const registering = mode === "register";
			$("#authEyebrow").textContent = registering ? "CREATE YOUR ACCOUNT" : "WELCOME BACK";
			$("#authTitle").textContent = registering ? "Join Return Trip" : "Sign in to Return Trip";
			$("#authIntro").textContent = registering ? "Set up your profile to get started with your shipment journey." : "Sign in to continue to your Return Trip account.";
			$("#authSubmit").innerHTML = registering ? 'Create account <span>→</span>' : 'Continue <span>→</span>';
			registerFields.classList.toggle("hidden", !registering);
			registerFields.querySelectorAll("input").forEach((input) => { input.disabled = !registering; input.required = registering; });
			roleSelect.disabled = !registering;
			authForm.querySelector("[name='password']").autocomplete = registering ? "new-password" : "current-password";
			$("#authSwitch").innerHTML = registering
				? 'Already registered? <button type="button" class="text-action" data-auth-open="login">Sign in</button>'
				: 'New to Return Trip? <button type="button" class="text-action" data-auth-open="register">Create an account</button>';
			feedback.classList.add("hidden");
			feedback.textContent = "";
			updateRegistrationRoleFields();
		}

		function openAuth(nextMode, role) {
			mode = nextMode;
			if (role && [...roleSelect.options].some((option) => option.value === role)) roleSelect.value = role;
			renderAuthMode();
			overlay.classList.remove("hidden");
			document.body.classList.add("auth-open");
			window.setTimeout(() => authForm.querySelector("input:not([disabled])")?.focus(), 40);
		}

		function closeAuth() {
			overlay.classList.add("hidden");
			document.body.classList.remove("auth-open");
		}

		document.addEventListener("click", (event) => {
			const open = event.target.closest("[data-auth-open]");
			if (open) {
				event.preventDefault();
				openAuth(open.dataset.authOpen, open.dataset.role);
				return;
			}
			if (event.target.closest("[data-auth-close]") || event.target === overlay) closeAuth();
		});
		document.addEventListener("keydown", (event) => { if (event.key === "Escape" && !overlay.classList.contains("hidden")) closeAuth(); });
		authForm.addEventListener("submit", (event) => {
			event.preventDefault();
			if (!authForm.reportValidity()) return;
			feedback.textContent = mode === "register"
				? `Your ${roleSelect.options[roleSelect.selectedIndex].text.toLowerCase()} registration is ready for the real account setup journey. This UI preview does not save account data yet.`
				: "Sign-in UI is ready to connect to authentication. No credentials were sent or saved in this preview.";
			feedback.classList.remove("hidden");
		});
		roleSelect.addEventListener("change", updateRegistrationRoleFields);
		updateRegistrationRoleFields();
	}

	bindPublicAuth();
	const publicNavToggle = $("#publicNavToggle");
	const publicNavLinks = $("#publicNavLinks");
	if (publicNavToggle && publicNavLinks) {
		publicNavToggle.addEventListener("click", () => {
			const expanded = publicNavToggle.getAttribute("aria-expanded") === "true";
			publicNavToggle.setAttribute("aria-expanded", String(!expanded));
			publicNavToggle.setAttribute("aria-label", expanded ? "Open navigation" : "Close navigation");
			publicNavLinks.classList.toggle("is-open", !expanded);
		});
		publicNavLinks.addEventListener("click", (event) => {
			if (!event.target.closest("a")) return;
			publicNavToggle.setAttribute("aria-expanded", "false");
			publicNavToggle.setAttribute("aria-label", "Open navigation");
			publicNavLinks.classList.remove("is-open");
		});
		document.addEventListener("keydown", (event) => {
			if (event.key !== "Escape") return;
			publicNavToggle.setAttribute("aria-expanded", "false");
			publicNavToggle.setAttribute("aria-label", "Open navigation");
			publicNavLinks.classList.remove("is-open");
		});
	}
	const landingRegisterForm = $("#register-form");
	if (landingRegisterForm) {
		const roleSelect = landingRegisterForm.querySelector("select[name='role']");
		const ownerFields = $("#vehicleOwnerFields");
		const driverFields = $("#driverFields");
		const feedback = $("#register-feedback");
		const syncRoleFields = () => {
			ownerFields.hidden = roleSelect.value !== "VehicleOwner";
			driverFields.hidden = roleSelect.value !== "Driver";
		};
		roleSelect.addEventListener("change", syncRoleFields);
		syncRoleFields();
		landingRegisterForm.addEventListener("submit", (event) => {
			event.preventDefault();
			if (!landingRegisterForm.reportValidity()) return;
			feedback.textContent = "Registration is ready to connect to account setup. This preview has not saved or sent your details.";
		});
	}
	const landingLoginForm = $("#login-form");
	if (landingLoginForm) {
		landingLoginForm.addEventListener("submit", (event) => {
			event.preventDefault();
			if (!landingLoginForm.reportValidity()) return;
			const note = landingLoginForm.querySelector(".modal-body small");
			if (note) note.textContent = "Login is ready to connect to authentication. Credentials were not sent or saved.";
		});
	}
	if (!form) return;

	const money = (value) => new Intl.NumberFormat("en-KE", { style: "currency", currency: "KES", maximumFractionDigits: 2 }).format(Number(value || 0));
	const dateTime = (value) => value ? new Intl.DateTimeFormat("en-KE", { dateStyle: "medium", timeStyle: "short" }).format(new Date(value)) : "Not set";
	const now = () => new Date();
	const plusHours = (hours) => new Date(Date.now() + hours * 3600000);
	const toLocalInput = (date) => {
		const local = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
		return local.toISOString().slice(0, 16);
	};
	const escapeHtml = (value) => String(value ?? "").replace(/[&<>"']/g, (char) => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" })[char]);
	const getPlace = (id) => places.find((place) => place.id === id);
	const selectedPriceMode = () => $("input[name='pricingMode']:checked").value;

	places.forEach((place) => {
		const option = `<option value="${place.id}">${escapeHtml(place.name)}</option>`;
		$("#cargoFrom").insertAdjacentHTML("beforeend", option);
		$("#cargoTo").insertAdjacentHTML("beforeend", option);
	});

	$("#requiredDepartureAt").value = toLocalInput(plusHours(24));
	$("#deliveryDeadlineAt").value = toLocalInput(plusHours(96));
	$("#truckEta").value = toLocalInput(plusHours(12));

	function log(message) {
		state.activity.unshift({ message, at: now() });
		renderActivity();
	}

	function renderActivity() {
		const feed = $("#activityFeed");
		$("#activityCount").textContent = state.activity.length;
		feed.innerHTML = state.activity.length
			? state.activity.map((entry) => `<li>${escapeHtml(entry.message)}<time>${dateTime(entry.at)}</time></li>`).join("")
			: '<li class="activity-empty">Journey events will appear here.</li>';
	}

	function setStage(stage) {
		state.stage = stage;
		$$('[data-stage-panel]').forEach((panel) => panel.classList.toggle("hidden", panel.dataset.stagePanel !== stage));
		$$('[data-step-nav]').forEach((button) => {
			const stages = ["post", "bid", "owner", "driver", "confirm"];
			const buttonStage = button.dataset.stepNav;
			button.classList.toggle("active", buttonStage === stage || (stage === "done" && buttonStage === "confirm"));
			button.classList.toggle("completed", stages.indexOf(buttonStage) < stages.indexOf(stage) || stage === "done");
		});
		$$('[data-dashboard-nav]').forEach((button) => button.classList.toggle("active", button.dataset.dashboardNav === stage));
		$("#journeySidebar")?.classList.remove("open");
		$("#journeySidebarOverlay")?.classList.remove("show");
		window.scrollTo({ top: 0, behavior: "smooth" });
		renderStage();
	}

	function renderStage() {
		renderSidebarCounts();
		renderKpis();
		renderSummary();
		if (state.stage === "bid") renderBidForm();
		if (state.stage === "owner") renderOwnerReview();
		if (state.stage === "driver") renderDriverDelivery();
		if (state.stage === "confirm") renderAgentReceiptReview();
		if (state.stage === "done") renderCompletion();
	}

	function renderSidebarCounts() {
		$("#sidebarTruckCount").textContent = state.availableTrucks.length || "—";
		$("#sidebarBidCount").textContent = state.bid && ["Pending", "AwaitingOwnerPriceApproval"].includes(state.bid.status) ? "1" : "—";
	}

	function renderKpis() {
		$("#kpiCargo").textContent = state.cargo?.status || "Draft";
		$("#kpiCargoSub").textContent = state.cargo ? state.cargo.title : "Not posted yet";
		$("#kpiTrucks").textContent = state.availableTrucks.length ? String(state.availableTrucks.length) : "—";
		$("#kpiBid").textContent = state.bid?.status || "Waiting";
		$("#kpiBidSub").textContent = state.bid ? `Offer ${money(state.bid.amount)}` : "No bid submitted";
		$("#kpiDelivery").textContent = state.receipt?.status || (state.bid?.status === "InTransit" ? "In transit" : "Not started");
		$("#kpiDeliverySub").textContent = state.receipt ? `Receipt from ${state.receipt.receiver}` : "Receipt not uploaded";
	}

	function renderSummary() {
		const summary = $("#journeySummary");
		const status = state.cargo?.status || "Draft";
		const statusClass = status === "Completed" ? "completed" : status === "Open" ? (state.cargo?.reopenedAt ? "reopened" : "open") : status === "InTransit" ? "transit" : status.includes("Pending") || status.includes("Approval") ? "pending" : status === "Accepted" ? "accepted" : "";
		$("#cargoStatus").textContent = status;
		$("#cargoStatus").className = `status-pill ${statusClass}`;
		if (!state.cargo) {
			summary.className = "journey-summary empty-summary";
			summary.innerHTML = '<div class="summary-empty-icon">↗</div><strong>Your journey starts here</strong><p>Post a cargo request to see route, price, and delivery details.</p>';
			$("#retentionControl").classList.add("hidden");
			return;
		}
		summary.className = "journey-summary";
		summary.innerHTML = `
			<div class="summary-route"><div class="summary-stop"><small>FROM</small><strong title="${escapeHtml(state.cargo.from.name)}">${escapeHtml(state.cargo.from.name.split(",")[0])}</strong></div><div class="summary-arrow">→</div><div class="summary-stop"><small>TO</small><strong title="${escapeHtml(state.cargo.to.name)}">${escapeHtml(state.cargo.to.name.split(",")[0])}</strong></div></div>
			<div class="summary-detail"><small>Transport total</small><strong>${money(state.cargo.transportCost)}</strong></div>
			<div class="summary-detail"><small>Must depart by</small><strong>${dateTime(state.cargo.requiredDepartureAt)}</strong></div>
			<div class="summary-detail"><small>Deliver by</small><strong>${dateTime(state.cargo.deliveryDeadlineAt)}</strong></div>
			${state.bid ? `<div class="summary-detail"><small>Offered truck</small><strong>${escapeHtml(state.bid.truck.plate)}</strong></div>` : ""}`;
		$("#retentionControl").classList.toggle("hidden", !state.cargo.retentionExpiresAt || state.cargo.status === "Completed" || state.cargo.status === "InTransit");
		$("#retentionDate").textContent = dateTime(state.cargo.retentionExpiresAt);
	}

	function updateLocations() {
		const from = getPlace($("#cargoFrom").value);
		const to = getPlace($("#cargoTo").value);
		$("#fromCoordinate").textContent = from ? `Demo map point · ${from.lat.toFixed(4)}, ${from.lng.toFixed(4)}` : "Select a place to show its coordinates";
		$("#toCoordinate").textContent = to ? `Demo map point · ${to.lat.toFixed(4)}, ${to.lng.toFixed(4)}` : "Select a place to show its coordinates";
		const validRoute = from && to && from.id !== to.id;
		$("#checkAvailability").disabled = !validRoute;
		$("#postCargo").disabled = !validRoute || !state.availableTrucks.length || !state.selectedTruckId;
		$("#cargoFormHint").textContent = !from || !to ? "Select pickup and delivery locations to check availability." : from.id === to.id ? "Pickup and delivery locations must be different." : state.availableTrucks.length ? "Select a matching truck above, then post this request." : "Check return-truck availability, then select a truck to post.";
	}

	function calculatePrice() {
		const perKg = selectedPriceMode() === "PerKg";
		$("#fixedPriceField").classList.toggle("hidden", perKg);
		$("#perKgPriceField").classList.toggle("hidden", !perKg);
		$("#calculatedTotal").classList.toggle("hidden", !perKg);
		$("#fixedPrice").required = !perKg;
		$("#pricePerKg").required = perKg;
		const weight = Number($("#cargoWeight").value);
		const rate = Number($("#pricePerKg").value);
		const total = weight > 0 && rate > 0 ? weight * rate : 0;
		$("#calculatedTotalValue").textContent = money(total);
		$("#calculatedFormula").textContent = `${weight || 0} kg × ${money(rate)} / kg`;
		$$("[data-pricing-choice]").forEach((choice) => choice.classList.toggle("selected", choice.dataset.pricingChoice === (perKg ? "PerKg" : "FixedTotal")));
		return perKg ? total : Number($("#fixedPrice").value);
	}

	function renderAvailability() {
		const from = getPlace($("#cargoFrom").value);
		const to = getPlace($("#cargoTo").value);
		const results = $("#availabilityResults");
		results.classList.remove("hidden");
		if (!from || !to || from.id === to.id) {
			results.innerHTML = '<div class="empty-result">Choose two different locations to search for trucks.</div>';
			return;
		}
		const requestedDeparture = new Date($("#requiredDepartureAt").value);
		state.availableTrucks = trucks.filter((truck) => {
			const timeForReturn = plusHours(truck.returnInHours);
			const returnEnds = new Date(timeForReturn.getTime() + truck.returnWindowHours * 3600000);
			return truck.currentTo === from.name && truck.currentFrom === to.name && requestedDeparture <= returnEnds && requestedDeparture >= plusHours(-2);
		});
		state.selectedTruckId = null;
		if (!state.availableTrucks.length) {
			results.innerHTML = `<div class="result-heading"><span>No trucks match this return route</span><span>0 found</span></div><div class="empty-result">The demo fleet has no truck returning ${escapeHtml(from.name.split(",")[0])} → ${escapeHtml(to.name.split(",")[0])} in the requested window. Try Nairobi → Mombasa or Nairobi → Nakuru.</div>`;
			updateLocations();
			renderSidebarCounts();
			renderKpis();
			return;
		}
		results.innerHTML = `<div class="result-heading"><span>Returning trucks that match your route</span><span>${state.availableTrucks.length} found</span></div>` + state.availableTrucks.map((truck) => `
			<label class="truck-option" data-truck-option="${truck.id}">
				<input type="radio" name="selectedTruck" value="${truck.id}" />
				<span class="truck-option-body"><span class="truck-option-top"><strong>${escapeHtml(truck.plate)} · ${escapeHtml(truck.type)}</strong><strong class="truck-match">Route match</strong></span>
				<span class="truck-route">Return load: ${escapeHtml(from.name.split(",")[0])} → ${escapeHtml(to.name.split(",")[0])}</span>
				<span class="truck-meta"><span>Current trip: ${escapeHtml(truck.currentFrom.split(",")[0])} → ${escapeHtml(truck.currentTo.split(",")[0])}</span><span>Available ${dateTime(plusHours(truck.returnInHours))}</span><span>Agent: ${escapeHtml(truck.agent)}</span></span></span>
			</label>`).join("");
		updateLocations();
		renderSidebarCounts();
		renderKpis();
	}

	function buildCargo() {
		const from = getPlace($("#cargoFrom").value);
		const to = getPlace($("#cargoTo").value);
		const mode = selectedPriceMode();
		const weight = Number($("#cargoWeight").value);
		const pricePerKg = mode === "PerKg" ? Number($("#pricePerKg").value) : null;
		const transportCost = calculatePrice();
		const departure = new Date($("#requiredDepartureAt").value);
		const delivery = new Date($("#deliveryDeadlineAt").value);
		const expires = new Date(departure.getTime() + Number($("#retentionDays").value) * 86400000);
		if (!from || !to || from.id === to.id) throw new Error("Choose different pickup and delivery places.");
		if (!state.availableTrucks.length || !state.selectedTruckId) throw new Error("Check availability and select a matching return truck first.");
		if (!Number.isFinite(departure.getTime()) || !Number.isFinite(delivery.getTime()) || delivery < departure) throw new Error("Delivery date must be on or after the must-depart date.");
		if (weight <= 0 || transportCost <= 0 || (mode === "PerKg" && (!pricePerKg || pricePerKg <= 0))) throw new Error("Enter a positive cargo weight and valid transport price.");
		return {
			title: $("#cargoTitle").value.trim(), description: $("#cargoDescription").value.trim(), from, to,
			weight, pricingMode: mode, pricePerKg, transportCost, requiredDepartureAt: departure,
			deliveryDeadlineAt: delivery, retentionDays: Number($("#retentionDays").value), retentionExpiresAt: expires,
			status: "Open", reopenedAt: null, departedAt: null, deliveredAt: null
		};
	}

	function renderCargoBrief() {
		const cargo = state.cargo;
		$("#agentCargoBrief").innerHTML = `<h3>${escapeHtml(cargo.title)}</h3><div class="brief-route">${escapeHtml(cargo.from.name)} → ${escapeHtml(cargo.to.name)}</div><div class="brief-meta"><span>${cargo.weight.toLocaleString()} kg</span><span>Must depart ${dateTime(cargo.requiredDepartureAt)}</span><span>Deliver by ${dateTime(cargo.deliveryDeadlineAt)}</span><span>Listed total ${money(cargo.transportCost)}</span></div>`;
		const truck = trucks.find((item) => item.id === state.selectedTruckId);
		$("#selectedTruckBrief").innerHTML = `<div><strong>${escapeHtml(truck.plate)} · ${escapeHtml(truck.type)}</strong><small>Agent ${escapeHtml(truck.agent)} · ${escapeHtml(truck.vehicleOwner)}</small></div><div><strong>${escapeHtml(truck.currentFrom.split(",")[0])} → ${escapeHtml(truck.currentTo.split(",")[0])}</strong><small>Returning for ${escapeHtml(cargo.from.name.split(",")[0])} → ${escapeHtml(cargo.to.name.split(",")[0])}</small></div>`;
	}

	function renderBidForm() {
		if (!state.cargo) { setStage("post"); return; }
		renderCargoBrief();
		$("#agentBidAmount").value = state.cargo.transportCost.toFixed(2);
		$("#agentPriceHint").textContent = `Listed price: ${money(state.cargo.transportCost)}`;
		$("#suggestionComments").value = "";
		$("#truckEta").value = toLocalInput(plusHours(12));
		updateBidSuggestionHint();
	}

	function updateBidSuggestionHint() {
		const proposed = Number($("#agentBidAmount").value);
		const differs = state.cargo && Math.abs(proposed - state.cargo.transportCost) >= 0.005;
		$("#suggestionComments").required = Boolean(differs);
		$("#bidRuleNote").textContent = differs
			? "This price differs from the listed total. Your comments are required, and the bid will wait for the goods owner’s approval or rejection."
			: "This matches the listed price. The bid goes to the goods owner for acceptance or rejection; the owner will also see the truck’s estimated arrival.";
	}

	function renderOwnerReview() {
		const host = $("#ownerBidReview");
		if (!state.bid) { host.innerHTML = '<div class="empty-result">No bid has been submitted yet.</div>'; return; }
		const suggestion = state.bid.suggestion;
		if (state.bid.status === "Released") {
			host.innerHTML = `<div class="warning-banner">The suggested price was rejected. This bid has been released; the agent may return to submit a new offer.</div><div class="review-actions"><button type="button" class="btn-approve" data-go-stage="bid">Return to agent bid</button><button type="button" class="btn-quiet" data-reset-journey>Reset simulation</button></div>`;
			return;
		}
		if (["Accepted", "InTransit", "DeliveredPendingConfirmation", "Completed"].includes(state.bid.status)) {
			host.innerHTML = `<div class="success-banner">${suggestion ? "Price suggestion approved and bid accepted." : "Bid accepted."} The owner has been notified of the truck’s estimated pickup arrival.</div><div class="review-card"><h3>${escapeHtml(state.cargo.title)}</h3><div class="review-price"><span>Agreed transport total</span><strong>${money(state.cargo.transportCost)}</strong></div><div class="review-price"><span>Truck arrival at pickup</span><strong>${dateTime(state.bid.eta)}</strong></div><div class="review-price"><span>Truck</span><strong>${escapeHtml(state.bid.truck.plate)}</strong></div></div><div class="review-actions"><button type="button" class="btn-approve" data-go-stage="driver">Continue to driver delivery →</button></div>`;
			return;
		}
		host.innerHTML = `<div class="review-card"><h3>${escapeHtml(state.cargo.title)}</h3><div class="review-price"><span>Listed transport total</span><strong>${money(state.bid.listedAmount)}</strong></div><div class="review-price"><span>${suggestion ? "Agent’s suggested total" : "Agent’s bid"}</span><strong>${money(state.bid.amount)}</strong></div><div class="review-price"><span>Truck arrival at pickup</span><strong>${dateTime(state.bid.eta)}</strong></div><div class="review-price"><span>Truck offered</span><strong>${escapeHtml(state.bid.truck.plate)}</strong></div>${suggestion ? `<div class="review-comments"><strong>Agent’s note:</strong> ${escapeHtml(state.bid.comments)}</div>` : ""}</div>
			<div class="review-actions"><button type="button" class="btn-approve" data-owner-decision="approve">${suggestion ? "Approve price & accept bid" : "Accept bid"}</button><button type="button" class="btn-reject" data-owner-decision="reject">${suggestion ? "Reject price & release bid" : "Reject bid"}</button></div>`;
	}

	function renderDriverDelivery() {
		const host = $("#driverDelivery");
		if (!state.bid || state.bid.status === "Released") { host.innerHTML = '<div class="empty-result">An accepted bid is needed before driver delivery can start.</div>'; return; }
		if (state.bid.status === "Accepted") {
			host.innerHTML = `<div class="success-banner">Bid accepted for ${money(state.cargo.transportCost)}. Simulated owner notification: ${dateTime(state.bid.eta)} vehicle arrival.</div><div class="driver-action"><div><strong>Ready to depart</strong><p>Mark the assigned driver’s load as in transit.</p></div><button type="button" class="btn-primary" data-mark-transit>Mark in transit →</button></div>`;
			return;
		}
		if (state.receipt?.status === "Pending") {
			host.innerHTML = `<div class="success-banner">Signed receipt submitted. Cargo is delivered and waiting for agent confirmation.</div>${receiptCard()}<div class="review-actions"><button type="button" class="btn-approve" data-go-stage="confirm">Open agent review →</button></div>`;
			return;
		}
		if (state.receipt?.status === "Rejected") {
			host.innerHTML = `<div class="warning-banner">The agent requested a new receipt: ${escapeHtml(state.receipt.agentComments || "Please upload a clearer signed receipt.")}</div>${receiptForm()}`;
			return;
		}
		host.innerHTML = `<div class="driver-action"><div><strong>Load in transit</strong><p>Departed ${dateTime(state.bid.departedAt)}. Delivery deadline: ${dateTime(state.cargo.deliveryDeadlineAt)}.</p></div><span class="status-pill transit">In transit</span></div>${receiptForm()}`;
	}

	function receiptForm() {
		return `<form class="delivery-card" id="receiptForm"><h3>Receiver’s signed delivery receipt</h3><div class="field-grid"><label class="field"><span>Receiver name</span><input class="form-control" id="receiverName" maxlength="100" placeholder="Person who received the goods" required /></label><label class="field"><span>Signed date and time</span><input class="form-control" id="receiptSignedAt" type="datetime-local" required /></label><div class="field field-wide"><span>Photo of signed receipt <small>Image only · max 5 MB</small></span><label class="upload-drop" for="receiptImage"><input id="receiptImage" type="file" accept="image/*" required /><span><span class="upload-icon">⇧</span><strong id="receiptFileLabel">Choose or take a photo</strong><small>JPG, PNG, HEIC or another supported image</small></span></label></div></div><div class="form-footer"><span class="form-hint">The bid completes only after the agent confirms this proof.</span><button type="submit" class="btn-primary">Submit receipt to agent <span>→</span></button></div></form>`;
	}

	function receiptCard() {
		const image = state.receipt?.previewUrl ? `<img src="${state.receipt.previewUrl}" alt="Receipt photo preview" />` : "▧";
		return `<div class="receipt-preview"><div class="receipt-thumb">${image}</div><div><strong>Signed receipt · ${escapeHtml(state.receipt?.fileName || "receipt photo")}</strong><small>Receiver: ${escapeHtml(state.receipt?.receiver || "Not recorded")} · Uploaded ${dateTime(state.receipt?.uploadedAt)}</small></div></div>`;
	}

	function renderAgentReceiptReview() {
		const host = $("#agentReceiptReview");
		if (!state.receipt) { host.innerHTML = '<div class="empty-result">The driver has not submitted a delivery receipt.</div><div class="review-actions"><button type="button" class="btn-quiet" data-go-stage="driver">Back to driver delivery</button></div>'; return; }
		if (state.receipt.status === "Confirmed") {
			host.innerHTML = `<div class="success-banner">Receipt confirmed. Cargo bid and shipment are marked complete.</div>${receiptCard()}<div class="review-actions"><button type="button" class="btn-approve" data-go-stage="done">View completion →</button></div>`;
			return;
		}
		if (state.receipt.status === "Rejected") {
			host.innerHTML = `<div class="warning-banner">Receipt needs a replacement photo from the driver.</div>${receiptCard()}<div class="review-actions"><button type="button" class="btn-approve" data-go-stage="driver">Return to driver upload</button></div>`;
			return;
		}
		host.innerHTML = `<div class="review-card"><h3>Delivery receipt review</h3><div class="review-price"><span>Cargo</span><strong>${escapeHtml(state.cargo.title)}</strong></div><div class="review-price"><span>Receiver</span><strong>${escapeHtml(state.receipt.receiver)}</strong></div><div class="review-price"><span>Signed at</span><strong>${dateTime(state.receipt.signedAt)}</strong></div><div class="review-price"><span>Driver</span><strong>Assigned driver · ${escapeHtml(state.bid.truck.plate)}</strong></div></div>${receiptCard()}<div class="review-actions"><button type="button" class="btn-approve" data-receipt-decision="confirm">Confirm receipt & complete</button><button type="button" class="btn-reject" data-receipt-decision="reject">Reject and request new photo</button></div>`;
	}

	function renderCompletion() {
		$("#completionSummary").innerHTML = `<div class="completion-summary"><div class="summary-route"><div class="summary-stop"><small>FROM</small><strong>${escapeHtml(state.cargo.from.name)}</strong></div><div class="summary-arrow">→</div><div class="summary-stop"><small>TO</small><strong>${escapeHtml(state.cargo.to.name)}</strong></div></div><div class="summary-detail"><small>Agreed total</small><strong>${money(state.cargo.transportCost)}</strong></div><div class="summary-detail"><small>Driver receipt</small><strong>${escapeHtml(state.receipt?.fileName || "Confirmed")}</strong></div><div class="summary-detail"><small>Completed</small><strong>${dateTime(state.bid.completedAt)}</strong></div></div>`;
	}

	function reset() {
		if (state.receiptObjectUrl) URL.revokeObjectURL(state.receiptObjectUrl);
		state.stage = "post";
		state.cargo = null;
		state.availableTrucks = [];
		state.selectedTruckId = null;
		state.bid = null;
		state.receipt = null;
		state.activity = [];
		state.receiptObjectUrl = null;
		form.reset();
		$("#cargoFrom").value = "";
		$("#cargoTo").value = "";
		$("#cargoWeight").value = "1000";
		$("#retentionDays").value = "2";
		$("#fixedPrice").value = "85000";
		$("#requiredDepartureAt").value = toLocalInput(plusHours(24));
		$("#deliveryDeadlineAt").value = toLocalInput(plusHours(96));
		$("#availabilityResults").innerHTML = "";
		$("#availabilityResults").classList.add("hidden");
		$("input[name='pricingMode'][value='FixedTotal']").checked = true;
		calculatePrice();
		updateLocations();
		renderActivity();
		setStage("post");
	}

	$("#cargoFrom").addEventListener("change", updateLocations);
	$("#cargoTo").addEventListener("change", updateLocations);
	$("#cargoWeight").addEventListener("input", calculatePrice);
	$("#pricePerKg").addEventListener("input", calculatePrice);
	$("#fixedPrice").addEventListener("input", calculatePrice);
	$$('input[name="pricingMode"]').forEach((input) => input.addEventListener("change", calculatePrice));
	$("#checkAvailability").addEventListener("click", renderAvailability);

	$("#availabilityResults").addEventListener("change", (event) => {
		if (event.target.name !== "selectedTruck") return;
		state.selectedTruckId = Number(event.target.value);
		$$('[data-truck-option]').forEach((item) => item.classList.toggle("selected", Number(item.dataset.truckOption) === state.selectedTruckId));
		updateLocations();
	});

	form.addEventListener("submit", (event) => {
		event.preventDefault();
		if (!form.reportValidity()) return;
		try {
			state.cargo = buildCargo();
			state.cargo.retentionExpiresAt = new Date(state.cargo.retentionExpiresAt);
			const truck = trucks.find((item) => item.id === state.selectedTruckId);
			state.bid = null;
			state.receipt = null;
			$("#availabilityResults").classList.add("hidden");
			log(`Cargo request posted: ${state.cargo.from.name.split(",")[0]} → ${state.cargo.to.name.split(",")[0]}.`);
			log(`Listed transport total ${money(state.cargo.transportCost)}; retention expires ${dateTime(state.cargo.retentionExpiresAt)}.`);
			log(`${truck.plate} is available for the return route; ${truck.agent} can now place a bid.`);
			setStage("bid");
		} catch (error) {
			window.alert(error.message);
		}
	});

	$("#bidForm").addEventListener("submit", (event) => {
		event.preventDefault();
		const bidForm = event.currentTarget;
		updateBidSuggestionHint();
		if (!bidForm.reportValidity()) return;
		const offered = Number($("#agentBidAmount").value);
		const differs = Math.abs(offered - state.cargo.transportCost) >= 0.005;
		const truck = trucks.find((item) => item.id === state.selectedTruckId);
		state.bid = {
			amount: offered,
			listedAmount: state.cargo.transportCost,
			suggestion: differs,
			comments: $("#suggestionComments").value.trim(),
			eta: new Date($("#truckEta").value),
			truck,
			status: differs ? "AwaitingOwnerPriceApproval" : "Pending",
			departedAt: null,
			completedAt: null
		};
		log(`${truck.agent} sent a ${differs ? "price suggestion" : "standard bid"} for ${money(offered)}.`);
		log(`Goods owner notified: truck arrival estimate ${dateTime(state.bid.eta)}.`);
		if (differs) log(`Owner approval request includes agent comments: “${state.bid.comments}”.`);
		setStage("owner");
	});

	$("#agentBidAmount").addEventListener("input", updateBidSuggestionHint);

	document.addEventListener("click", (event) => {
		const step = event.target.closest("[data-step-nav]");
		const sideNav = event.target.closest("[data-dashboard-nav]");
		const go = event.target.closest("[data-go-stage]");
		const ownerDecision = event.target.closest("[data-owner-decision]");
		const receiptDecision = event.target.closest("[data-receipt-decision]");
		if (step) setStage(step.dataset.stepNav);
		if (sideNav) setStage(sideNav.dataset.dashboardNav);
		if (go) setStage(go.dataset.goStage);

		if (ownerDecision && state.bid) {
			if (ownerDecision.dataset.ownerDecision === "reject") {
				state.bid.status = "Released";
				if (state.bid.suggestion) {
					state.bid.priceSuggestionStatus = "Rejected";
					log("Goods owner rejected the suggested price; bid released back to the market.");
				} else {
					log("Goods owner rejected the agent’s bid.");
				}
			} else {
				if (state.bid.suggestion) {
					state.bid.priceSuggestionStatus = "Approved";
					state.cargo.transportCost = state.bid.amount;
					log(`Goods owner approved the price suggestion. Cargo price updated to ${money(state.cargo.transportCost)}.`);
				} else {
					log(`Goods owner accepted the bid at ${money(state.bid.amount)}.`);
				}
				state.bid.status = "Accepted";
				state.bid.acceptedAt = now();
				state.cargo.status = "Accepted";
				log(`Goods owner notified: ${state.bid.truck.plate} arrival at pickup is estimated for ${dateTime(state.bid.eta)}.`);
			}
			renderStage();
		}

		if (receiptDecision && state.receipt) {
			if (receiptDecision.dataset.receiptDecision === "confirm") {
				state.receipt.status = "Confirmed";
				state.receipt.confirmedAt = now();
				state.bid.status = "Completed";
				state.bid.completedAt = now();
				state.cargo.status = "Completed";
				state.cargo.deliveredAt = state.receipt.signedAt;
				log("Agent confirmed the signed receipt; the bid and cargo are complete.");
				setStage("done");
			} else {
				state.receipt.status = "Rejected";
				state.receipt.agentComments = "Please upload a clearer image showing the receiver’s signature.";
				state.bid.status = "InTransit";
				state.cargo.status = "InTransit";
				log("Agent could not verify the receipt and requested a replacement photo from the driver.");
				setStage("driver");
			}
			renderStage();
		}

		if (event.target.closest("[data-reset-journey]")) reset();
	});

	document.addEventListener("click", (event) => {
		if (!event.target.closest("[data-mark-transit]")) return;
		state.bid.status = "InTransit";
		state.bid.departedAt = now();
		state.cargo.status = "InTransit";
		state.cargo.departedAt = state.bid.departedAt;
		log(`Driver marked ${state.cargo.title} in transit; departure recorded.`);
		setStage("driver");
	});

	document.addEventListener("change", (event) => {
		if (event.target.id !== "receiptImage") return;
		const file = event.target.files?.[0];
		if (!file) return;
		if (!file.type.startsWith("image/")) {
			window.alert("Choose an image file for the signed receipt.");
			event.target.value = "";
			return;
		}
		if (file.size > 5 * 1024 * 1024) {
			window.alert("The receipt photo must be 5 MB or smaller.");
			event.target.value = "";
			return;
		}
		$("#receiptFileLabel").textContent = file.name;
	});

	document.addEventListener("submit", (event) => {
		if (event.target.id !== "receiptForm") return;
		event.preventDefault();
		const receiptFormElement = event.target;
		if (!receiptFormElement.reportValidity()) return;
		const file = $("#receiptImage").files?.[0];
		if (!file) { window.alert("Upload a photo of the signed receipt."); return; }
		if (state.receiptObjectUrl) URL.revokeObjectURL(state.receiptObjectUrl);
		state.receiptObjectUrl = URL.createObjectURL(file);
		state.receipt = {
			receiver: $("#receiverName").value.trim(),
			signedAt: new Date($("#receiptSignedAt").value),
			uploadedAt: now(),
			fileName: file.name,
			previewUrl: state.receiptObjectUrl,
			status: "Pending"
		};
		state.bid.status = "DeliveredPendingConfirmation";
		state.cargo.status = "DeliveredPendingConfirmation";
		state.cargo.deliveredAt = state.receipt.signedAt;
		log(`Driver uploaded the signed receipt photo for receiver ${state.receipt.receiver}.`);
		log("Agent notified to review and confirm the delivery receipt.");
		setStage("confirm");
	});

	$("#simulateExpiry").addEventListener("click", () => {
		if (!state.cargo || state.cargo.departedAt || state.cargo.status === "Completed") {
			window.alert("Only undeparted cargo can return to the market at retention expiry.");
			return;
		}
		if (state.bid && state.bid.status !== "Completed") {
			state.bid.status = "Released";
			log("Active bid released because the cargo retention period expired before departure.");
		}
		state.cargo.status = "Open";
		state.cargo.reopenedAt = now();
		log("Retention expired. Cargo returned to the open market for new bids.");
		setStage("post");
	});

	$("#resetDemo").addEventListener("click", reset);
	$("#resetDemoTop")?.addEventListener("click", reset);
	$("#startAnother").addEventListener("click", reset);
	$("#journeyMenuToggle")?.addEventListener("click", () => {
		$("#journeySidebar").classList.toggle("open");
		$("#journeySidebarOverlay").classList.toggle("show");
	});
	$("#journeySidebarOverlay")?.addEventListener("click", () => {
		$("#journeySidebar").classList.remove("open");
		$("#journeySidebarOverlay").classList.remove("show");
	});

	calculatePrice();
	updateLocations();
	renderActivity();
	renderStage();
})();
